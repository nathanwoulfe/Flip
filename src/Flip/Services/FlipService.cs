using System.Reflection;
using Flip.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Flip.Services;

internal sealed class FlipService : IFlipService
{
    private readonly IContentTypeService _contentTypeService;
    private readonly IContentService _contentService;
    private readonly ILanguageService _languageService;
    private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
    private readonly IIdKeyMap _idKeyMap;

    public FlipService(
        IContentTypeService contentTypeService,
        IContentService contentService,
        IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
        ILanguageService languageService,
        IIdKeyMap idKeyMap)
    {
        _contentTypeService = contentTypeService;
        _contentService = contentService;
        _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        _languageService = languageService;
        _idKeyMap = idKeyMap;
    }

    public async Task<bool> TryChangeContentType(ChangeDocumentTypeResponseModel model)
    {
        IContent? node = _contentService.GetById(model.Unique);

        if (node is null)
        {
            //message = "Could not find source content";
            return false;
        }

        if (node.ContentType.Id == model.ContentTypeId)
        {
            //message = "Current type and target type are the same";
            return false;
        }

        IContentType? newType = _contentTypeService.GetAll().FirstOrDefault(x => x.Id == model.ContentTypeId);
        IEnumerable<ILanguage> languages = await _languageService.GetAllAsync();

        Dictionary<string, string>? cultureNames = [];

        if (node.ContentType.VariesByCulture())
        {
            foreach (ILanguage language in languages)
            {
                cultureNames.Add(language.IsoCode, node.GetCultureName(language.IsoCode) ?? node.Name!);
            }
        }

        if (newType is null)
        {
            //message = "Could not find target content type";
            return false;
        }

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
            DocumentTypePropertyResponseModel? newProp = model.Properties?.FirstOrDefault(p => p.NewAlias == prop.Alias);

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

        //message = "OK";
        return true;
    }

    public ChangeDocumentTypeResponseModel GetContentModel(Guid unique)
    {
        IContent? content = _contentService.GetById(unique) ?? throw new NullReferenceException(nameof(content));

        ChangeDocumentTypeResponseModel model = new()
        {
            Unique = unique,
            NodeName = content.Name,
            TemplateId = content.TemplateId,
            ContentTypeId = content.ContentTypeId,
            ContentTypeName = content.ContentType.Name,
            Properties = content.Properties.Select(p => new DocumentTypePropertyResponseModel()
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

    public IEnumerable<IContentType> GetPermittedTypes(Guid unique)
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
        permittedTypes = RemoveInvalidByChildrenDocumentTypesFromAlternatives(permittedTypes, unique);

        return permittedTypes;
    }

    private static IEnumerable<IContentType> RemoveCurrentDocumentTypeFromAlternatives(IEnumerable<IContentType> documentTypes, int currentTypeId) =>
        documentTypes.Where(x => x.Id != currentTypeId);

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

    private IEnumerable<IContentType> RemoveInvalidByChildrenDocumentTypesFromAlternatives(IEnumerable<IContentType> documentTypes, Guid unique)
    {
        Attempt<int> idAttempt = _idKeyMap.GetIdForKey(unique, UmbracoObjectTypes.Document);
        if (idAttempt.Success is false)
        {
            return [];
        }

        IEnumerable<IContent> children = _contentService.GetPagedChildren(idAttempt.Result, 0, 10000, out _);

        IEnumerable<Guid> docTypeIdsOfChildren = children
            .Select(x => x.ContentType.Key)
            .Distinct();

        return documentTypes
            .Where(x => x.AllowedContentTypes is not null && x.AllowedContentTypes
                .Select(y => y.Key)
                .ContainsAll(docTypeIdsOfChildren));
    }
}
