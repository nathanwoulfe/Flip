using System.Reflection;
using Flip.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Flip.Services.Implement;

internal sealed class FlipService : IFlipService
{
    private readonly IContentTypeService _contentTypeService;
    private readonly IContentService _contentService;
    private readonly ILocalizationService _localizationService;
    private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

    public FlipService(
        IContentTypeService contentTypeService,
        IContentService contentService,
        IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
        ILocalizationService localizationService)
    {
        _contentTypeService = contentTypeService;
        _contentService = contentService;
        _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        _localizationService = localizationService;
    }

    /// <inheritdoc/>
    public bool TryChangeContentType(ChangeDocumentTypeModel model, out string message)
    {
        IContent? node = _contentService.GetById(model.NodeId);

        if (node is null)
        {
            message = "Could not find source content";
            return false;
        }

        if (node.ContentType.Id == model.ContentTypeId)
        {
            message = "Current type and target type are the same";
            return false;
        }

        IContentType? newType = _contentTypeService.GetAll().FirstOrDefault(x => x.Id == model.ContentTypeId);
        IEnumerable<ILanguage> languages = _localizationService.GetAllLanguages();

        Dictionary<string, string>? cultureNames = new();

        if (node.ContentType.VariesByCulture())
        {
            foreach (ILanguage language in languages)
            {
                cultureNames.Add(language.IsoCode, node.GetCultureName(language.IsoCode) ?? node.Name!);
            }
        }

        if (newType is null)
        {
            message = "Could not find target content type";
            return false;
        }

        MethodInfo? changeContentType = node.GetType()
            .GetMethod(
                "ChangeContentType",
                types: new[] { typeof(IContentType), typeof(bool) },
                modifiers: null,
                binder: null,
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

        _ = changeContentType?.Invoke(node, new object[] { newType, true });

        node.TemplateId = model.TemplateId;

        // ensure properties are cleared and re-mapped
        foreach (IProperty prop in node.Properties)
        {
            DocumentTypePropertyModel? newProp = model.Properties?.FirstOrDefault(p => p.NewAlias == prop.Alias);

            // if no values, set default to null
            if (newProp?.Values is null)
            {
                node.SetValue(prop.Alias, null);
                continue;
            }

            // only iterate when the property varies,
            // otherwise set the single value with a null culture
            if (prop.PropertyType.VariesByCulture())
            {
                foreach (ILanguage language in languages)
                {
                    (string? culture, object? value) = newProp.Values.Count() == 1 ? newProp.Values.First() : newProp.Values.FirstOrDefault(x => x.Culture == language.IsoCode);
                    node.SetValue(prop.Alias, value ?? null, language.IsoCode);
                }

                continue;
            }

            // use the first item if exists, else fall back to null value
            node.SetValue(prop.Alias, newProp.Values.Any() ? newProp.Values.First().Value : null);
        }

        if (newType.VariesByCulture())
        {
            foreach (ILanguage lang in languages)
            {
                KeyValuePair<string, string> existingName = cultureNames.FirstOrDefault(x => x.Key == lang.IsoCode);
                node.SetCultureName(existingName.Value ?? node.Name, lang.IsoCode);
            }
        }

        _ = _contentService.Save(node, _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?.Id);

        message = "OK";
        return true;
    }

    /// <inheritdoc />
    public ChangeDocumentTypeModel GetContentModel(int nodeId)
    {
        IContent? content = _contentService.GetById(nodeId) ?? throw new Exception();

        ChangeDocumentTypeModel model = new()
        {
            NodeId = nodeId,
            NodeName = content.Name,
            TemplateId = content.TemplateId,
            ContentTypeId = content.ContentTypeId,
            ContentTypeName = content.ContentType.Name,
            Properties = content.Properties.Select(p => new DocumentTypePropertyModel()
            {
                Alias = p.Alias,
                Label = p.PropertyType.Name,
                Editor = p.PropertyType.PropertyEditorAlias,
                DataTypeKey = p.PropertyType.DataTypeKey.ToString(),
                Value = p.GetValue(),
                Values = p.Values.Select(v => (v.Culture, Value: v.EditedValue)),
            }),
        };

        return model;
    }

    /// <inheritdoc />
    public IEnumerable<IContentType> GetPermittedTypes(int nodeId)
    {
        IContent? content = _contentService.GetById(nodeId);

        if (content is null)
        {
            return Enumerable.Empty<IContentType>();
        }

        IEnumerable<IContentType> permittedTypes = _contentTypeService.GetAll();

        if (!permittedTypes.Any())
        {
            return Enumerable.Empty<IContentType>();
        }

        permittedTypes = RemoveCurrentDocumentTypeFromAlternatives(permittedTypes, content.ContentTypeId);
        permittedTypes = RemoveInvalidByParentDocumentTypesFromAlternatives(permittedTypes, content.ParentId);
        permittedTypes = RemoveInvalidByChildrenDocumentTypesFromAlternatives(permittedTypes, nodeId);

        return permittedTypes;
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
            return Enumerable.Empty<IContentType>();
        }

        IContentType? parentType = _contentTypeService.Get(parentNode.ContentTypeId);

        if (parentType is null)
        {
            return Enumerable.Empty<IContentType>();
        }

        return documentTypes
            .Where(x => parentType.AllowedContentTypes is not null && parentType.AllowedContentTypes
                .Select(y => y.Id.Value)
                .Contains(x.Id));
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

        IEnumerable<int> docTypeIdsOfChildren = children
            .Select(x => x.ContentType.Id)
            .Distinct();

        return documentTypes
            .Where(x => x.AllowedContentTypes is not null && x.AllowedContentTypes
                .Select(y => y.Id.Value)
                .ContainsAll(docTypeIdsOfChildren));
    }
}
