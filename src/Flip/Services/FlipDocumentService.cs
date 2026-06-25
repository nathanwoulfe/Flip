using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace Flip.Services;

internal class FlipDocumentService(
    IContentService contentService,
    IIdKeyMap idKeyMap,
    IContentTypeService contentTypeService,
    IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
    : FlipService<IContent>(
        idKeyMap,
        contentTypeService,
        backOfficeSecurityAccessor)
{
    protected override IPublishableContentService<IContent> EntityService { get; } = contentService;

    protected override UmbracoObjectTypes EntityObjectType => UmbracoObjectTypes.Document;
}
