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
    private readonly IDataTypeService _dataTypeService;

    public FlipService(
        IContentTypeService contentTypeService,
        IContentService contentService,
        IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
        ILanguageService languageService,
        IIdKeyMap idKeyMap,
        IDataTypeService dataTypeService)
    {
        _contentTypeService = contentTypeService;
        _contentService = contentService;
        _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        _languageService = languageService;
        _idKeyMap = idKeyMap;
        _dataTypeService = dataTypeService;
    }

    public async Task<Attempt<bool, string>> ChangeDocumentType(ChangeDocumentTypeRequestModel model)
    {
        IContent? node = _contentService.GetById(model.Unique);

        if (node is null)
        {
            return Attempt.FailWithStatus("Could not find source content", false);
        }

        if (node.ContentType.Key == model.DocumentTypeUnique)
        {
            return Attempt.FailWithStatus("Current type and target type are the same", false);
        }

        IContentType? newType = _contentTypeService.GetAll().FirstOrDefault(x => x.Key == model.DocumentTypeUnique);
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
            return Attempt.FailWithStatus("Could not find the target content type", false);
        }

        var oldProperties = node.Properties.DeepClone() as IPropertyCollection;

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
            // get the original property by looking up the original alias via the new.
            DocumentTypePropertyResponseModel? mappedProp = model.Properties?.FirstOrDefault(p => p.NewAlias == prop.Alias);
            if (mappedProp is null)
            {
                node.SetValue(prop.Alias, null);
                continue;
            }

            IProperty? oldProp = oldProperties?.FirstOrDefault(x => x.Alias == mappedProp?.Alias);

            // if no values, set default to null
            if (oldProp?.Values is null)
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
                    IPropertyValue? value = oldProp.Values.Count == 1 ? oldProp.Values.First() : oldProp.Values.FirstOrDefault(x => x.Culture == language.IsoCode);
                    node.SetValue(prop.Alias, value, language.IsoCode);
                }

                continue;
            }

            // use the first item if exists, else fall back to null value
            node.SetValue(prop.Alias, oldProp.GetValue());
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

        return Attempt.SucceedWithStatus(string.Empty, true);
    }

    public async Task<ContentModelResponseModel> GetContentModel(Guid unique)
    {
        IContent? content = _contentService.GetById(unique) ?? throw new NullReferenceException(nameof(content));
        IEnumerable<IDataType> dataTypes = await _dataTypeService.GetAllAsync(content.Properties.Select(prop => prop.PropertyType.DataTypeKey).ToArray());

        ContentModelResponseModel model = new()
        {
            Unique = unique,
            Name = content.Name,
            TemplateId = content.TemplateId,
            DocumentType = new()
            {
                Unique = content.ContentType.Key,
                Name = content.ContentType.Name ?? content.ContentType.Alias,
                Alias = content.ContentType.Alias,
                Icon = content.ContentType.Icon,
            },
            Properties = content.Properties.Select(p => new DocumentTypePropertyResponseModel()
            {
                Alias = p.Alias,
                Label = p.PropertyType.Name,
                DataTypeKey = p.PropertyType.DataTypeKey,
                Editor = dataTypes.First(x => x.Key == p.PropertyType.DataTypeKey)?.EditorUiAlias,
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
