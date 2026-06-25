using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace Flip.Services;

internal class FlipElementService(
    IElementService elementService,
    IIdKeyMap idKeyMap,
    IContentTypeService contentTypeService,
    IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
    : FlipService<IElement>(
        idKeyMap,
        contentTypeService,
        backOfficeSecurityAccessor)
{
    protected override IPublishableContentService<IElement> EntityService { get; } = elementService;

    protected override UmbracoObjectTypes EntityObjectType => UmbracoObjectTypes.Element;
}
