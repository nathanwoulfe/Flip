using System.Reflection;
using Flip.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Flip.Services;

internal sealed class FlipService : IFlipService
{
    private readonly IContentTypeService _contentTypeService;
    private readonly IContentService _contentService;
    private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

    public FlipService(
        IContentTypeService contentTypeService,
        IContentService contentService,
        IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
    {
        _contentTypeService = contentTypeService;
        _contentService = contentService;
        _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
    }

    /// <inheritdoc/>
    public bool TryChangeContentType(ChangeDocumentTypeModel model, out string? message)
    {
        message = null;
        IContent? node = _contentService.GetById(model.Unique);

        if (node is null)
        {
            message = "Could not find source content";
            return false;
        }

        if (node.ContentType.Key == model.ContentTypeKey)
        {
            message = "Current type and target type are the same";
            return false;
        }

        IContentType? newType = _contentTypeService.GetAll().FirstOrDefault(x => x.Key == model.ContentTypeKey);

        Dictionary<string, string>? cultureNames = [];

        if (node.ContentType.VariesByCulture())
        {
            foreach (string culture in node.AvailableCultures)
            {
                cultureNames.Add(culture, node.GetCultureName(culture) ?? node.Name!);
            }
        }

        if (newType is null)
        {
            message = "Could not find target content type";
            return false;
        }

        var properties = node.Properties
            .Where(p => p.Alias is not null)
            .Select(p => new DocumentTypePropertyModel()
            {
                Alias = p.Alias,
                Label = p.PropertyType.Name,
                Editor = p.PropertyType.PropertyEditorAlias,
                DataTypeKey = p.PropertyType.DataTypeKey.ToString(),
                Value = p.GetValue(),
                Values = p.Values.Select(v => (v.Culture, Value: v.EditedValue)),
            }).ToDictionary(x => x.Alias!);

        MethodInfo? changeContentType = node.GetType()
            .GetMethod(
                "ChangeContentType",
                types: [typeof(IContentType), typeof(bool)],
                modifiers: null,
                binder: null,
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        _ = changeContentType?.Invoke(node, [newType, true]);

        node.TemplateId = model.TemplateId;

        // ensure properties are cleared and re-mapped
        foreach (IProperty prop in node.Properties)
        {
            // finds the new property to map to
            DocumentTypePropertyModel? newProp = model.Properties?.FirstOrDefault(p => p.NewAlias == prop.Alias);
            if (newProp is null)
            {
                continue;
            }

            // and the original value before we switch the type
            DocumentTypePropertyModel? oldProp = properties[prop.Alias];
            newProp.Values = oldProp?.Values;
            newProp.Value = oldProp?.Value;

            // if no values, set default to null
            if (newProp.Values is null)
            {
                node.SetValue(prop.Alias, null);
                continue;
            }

            // only iterate when the property varies,
            // otherwise set the single value with a null culture
            if (prop.PropertyType.VariesByCulture())
            {
                foreach (string culture in node.AvailableCultures)
                {
                    (string? _, object? value) = newProp.Values.Count() == 1 ? newProp.Values.First() : newProp.Values.FirstOrDefault(x => x.Culture == culture);
                    node.SetValue(prop.Alias, value ?? null, culture);
                }

                continue;
            }

            // use the first item if exists, else fall back to null value
            node.SetValue(prop.Alias, newProp.Values.Any() ? newProp.Values.First().Value : null);
        }

        if (newType.VariesByCulture())
        {
            foreach (string culture in node.AvailableCultures)
            {
                KeyValuePair<string, string> existingName = cultureNames.FirstOrDefault(x => x.Key == culture);
                node.SetCultureName(existingName.Value ?? node.Name, culture);
            }
        }

        _ = _contentService.Save(node, _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?.Id);

        return true;
    }

    /// <inheritdoc />
    public ChangeDocumentTypeModel? GetContentModel(Guid unique)
    {
        IContent? content = _contentService.GetById(unique);
        if (content is null)
        {
            return null;
        }

        ChangeDocumentTypeModel model = new()
        {
            Properties = content.Properties.Select(p => new DocumentTypePropertyModel()
            {
                Alias = p.Alias,
                Label = p.PropertyType.Name,
                Editor = p.PropertyType.PropertyEditorAlias,
                DataTypeKey = p.PropertyType.DataTypeKey.ToString(),
            }),
        };

        return model;
    }

    /// <inheritdoc />
    public IEnumerable<ContentTypeModel> GetPermittedTypes(Guid unique)
    {
        IContent? content = _contentService.GetById(unique);

        if (content is null)
        {
            return [];
        }

        IEnumerable<IContentType> permittedTypes = _contentTypeService.GetAll();

        if (!permittedTypes.Any())
        {
            return [];
        }

        permittedTypes = RemoveCurrentDocumentTypeFromAlternatives(permittedTypes, content.ContentTypeId);
        permittedTypes = RemoveInvalidByParentDocumentTypesFromAlternatives(permittedTypes, content.ParentId);
        permittedTypes = RemoveInvalidByChildrenDocumentTypesFromAlternatives(permittedTypes, content.Id);

        return permittedTypes.Select(x => new ContentTypeModel()
        {
            Name = x.Name,
            Unique = x.Key,
            DefaultTemplateId = x.DefaultTemplateId,
            PropertyTypes = x.CompositionPropertyTypes.Select(y => new PropertyTypeModel()
            {
                Name = y.Name,
                Alias = y.Alias,
                DataTypeKey = y.DataTypeKey,
                PropertyEditorAlias = y.PropertyEditorAlias,
            }),
            AllowedTemplates = x.AllowedTemplates?.Select(y => new TemplateModel()
            {
                Name = y.Name,
                Id = y.Id,
            }) ?? [],
        });
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="documentTypes"></param>
    /// <param name="currentTypeId"></param>
    /// <returns></returns>
    private static IEnumerable<IContentType> RemoveCurrentDocumentTypeFromAlternatives(IEnumerable<IContentType> documentTypes, int currentTypeId) =>
        documentTypes.Where(x => x.Id != currentTypeId);

    /// <summary>
    ///
    /// </summary>
    /// <param name="documentTypes"></param>
    /// <param name="parentId"></param>
    /// <returns></returns>
    private IEnumerable<IContentType> RemoveInvalidByParentDocumentTypesFromAlternatives(IEnumerable<IContentType> documentTypes, int parentId)
    {
        if (parentId == -1)
        {
            // Root content, only include those that have been selected as allowed at root
            return documentTypes.Where(x => x.AllowedAsRoot);
        }

        // Below root, so only include those allowed as sub-nodes for the parent
        IContent? parentNode = _contentService.GetById(parentId);

        if (parentNode is null)
        {
            return [];
        }

        IContentType? parentType = _contentTypeService.Get(parentNode.ContentTypeId);

        if (parentType is null)
        {
            return [];
        }

        return documentTypes
            .Where(x => parentType.AllowedContentTypes is not null && parentType.AllowedContentTypes
                .Select(y => y.Key)
                .Contains(x.Key));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="documentTypes"></param>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    private IEnumerable<IContentType> RemoveInvalidByChildrenDocumentTypesFromAlternatives(IEnumerable<IContentType> documentTypes, int nodeId)
    {
        IEnumerable<IContent> children = _contentService.GetPagedChildren(nodeId, 0, 10000, out _);

        IEnumerable<Guid> docTypeIdsOfChildren = children
            .Select(x => x.ContentType.Key)
            .Distinct();

        return documentTypes
            .Where(x => x.AllowedContentTypes is not null && x.AllowedContentTypes
                .Select(y => y.Key)
                .ContainsAll(docTypeIdsOfChildren));
    }
}
