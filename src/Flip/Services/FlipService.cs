using System.Reflection;
using Flip.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Flip.Services;

internal abstract class FlipService<TEntity>(
    IIdKeyMap idKeyMap,
    IContentTypeService contentTypeService,
    IBackOfficeSecurityAccessor backOfficeSecurityAccessor) : IFlipService<TEntity>
    where TEntity : class, IPublishableContentBase
{
    protected abstract IPublishableContentService<TEntity> EntityService { get; }

    protected abstract UmbracoObjectTypes EntityObjectType { get; }

    /// <inheritdoc/>
    public bool TryChangeContentType(ChangeEntityTypeModel model, out string? message)
    {
        message = null;

        if (EntityService.GetById(model.Unique) is not TEntity node)
        {
            message = "Could not find source content";
            return false;
        }

        if (node.ContentType.Key == model.ContentTypeKey)
        {
            message = "Current type and target type are the same";
            return false;
        }

        if (contentTypeService.GetAll().FirstOrDefault(x => x.Key == model.ContentTypeKey) is not IContentType newType)
        {
            message = "Could not find target content type";
            return false;
        }

        Dictionary<string, string>? cultureNames = [];

        if (node.ContentType.VariesByCulture())
        {
            foreach (string culture in node.AvailableCultures)
            {
                cultureNames.Add(culture, node.GetCultureName(culture) ?? node.Name!);
            }
        }

        var properties = node.Properties
            .Where(p => p.Alias is not null)
            .Select(p => new EntityTypePropertyModel()
            {
                Alias = p.Alias,
                Label = p.PropertyType.Name,
                Editor = p.PropertyType.PropertyEditorAlias,
                DataTypeKey = p.PropertyType.DataTypeKey,
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

        if (node is IContent contentNode)
        {
            contentNode.TemplateId = model.TemplateKey.HasValue ? idKeyMap.GetIdForKey(model.TemplateKey.Value, UmbracoObjectTypes.Template).Result : 0;
        }

        // ensure properties are cleared and re-mapped
        foreach (IProperty prop in node.Properties)
        {
            // finds the new property to map to
            if (model.Properties?.FirstOrDefault(p => p.NewAlias == prop.Alias) is not EntityTypePropertyModel newProp)
            {
                continue;
            }

            // and the original value before we switch the type
            EntityTypePropertyModel? oldProp = properties[prop.Alias];
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

        _ = EntityService.Save(node, backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?.Id);

        return true;
    }

    /// <inheritdoc />
    public ChangeEntityTypeModel? GetContentModel(Guid unique)
    {
        if (EntityService.GetById(unique) is not TEntity content)
        {
            return null;
        }

        ChangeEntityTypeModel model = new()
        {
            Unique = content.Key,
            ContentTypeKey = content.ContentType.Key,
            Properties = content.Properties.Select(p => new EntityTypePropertyModel()
            {
                Alias = p.Alias,
                Label = p.PropertyType.Name,
                Editor = p.PropertyType.PropertyEditorAlias,
                DataTypeKey = p.PropertyType.DataTypeKey,
            }),
        };

        return model;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ContentTypeModel>> GetPermittedTypes(Guid unique)
    {
        if (EntityService.GetById(unique) is not TEntity content)
        {
            return [];
        }

        IEnumerable<IContentType> permittedTypes = contentTypeService
            .GetAll()
            .Where(x => content is IContent ? !x.IsElement : x.IsElement);

        if (!permittedTypes.Any())
        {
            return [];
        }

        permittedTypes = RemoveCurrentDocumentTypeFromAlternatives(permittedTypes, content.ContentTypeId);

        if (content is IContent)
        {
            permittedTypes = RemoveInvalidByParentDocumentTypesFromAlternatives(permittedTypes, content.ParentId);
            permittedTypes = await RemoveInvalidByChildrenDocumentTypesFromAlternatives(permittedTypes, content.ContentType.Key);
        }
        else
        {
            permittedTypes = RemoveInvalidElementsFromAlternatives(permittedTypes, (IElement)content);
        }

        return permittedTypes.Select(x => new ContentTypeModel()
        {
            Name = x.Name,
            Unique = x.Key,
            DefaultTemplateKey = x.DefaultTemplate?.Key,
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
                Key = y.Key,
            }) ?? [],
        });
    }

    private IEnumerable<IContentType> RemoveInvalidElementsFromAlternatives(IEnumerable<IContentType> documentTypes, IElement element)
    {
        bool allowedInLibrary = element.ContentType.AllowedInLibrary;
        return documentTypes.Where(x => x.AllowedInLibrary == allowedInLibrary);
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
        Attempt<Guid> parentKeyAttempt = idKeyMap.GetKeyForId(parentId, EntityObjectType);
        if (!parentKeyAttempt.Success)
        {
            return [];
        }

        if (EntityService.GetById(parentKeyAttempt.Result) is not TEntity parentNode)
        {
            return [];
        }

        if (contentTypeService.Get(parentNode.ContentTypeId) is not IContentType parentType)
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
    /// <param name="nodeKey"></param>
    /// <returns></returns>
    private async Task<IEnumerable<IContentType>> RemoveInvalidByChildrenDocumentTypesFromAlternatives(IEnumerable<IContentType> documentTypes, Guid nodeKey)
    {
        if (await contentTypeService.GetAllowedChildrenAsync(nodeKey, 0, int.MaxValue) is not { Success: true, Result: { } children })
        {
            return [];
        }

        IEnumerable<Guid> docTypeIdsOfChildren = children.Items
            .Select(x => x.Key)
            .Distinct();

        return documentTypes
            .Where(x => x.AllowedContentTypes is not null && x.AllowedContentTypes
                .Select(y => y.Key)
                .ContainsAll(docTypeIdsOfChildren));
    }
}
