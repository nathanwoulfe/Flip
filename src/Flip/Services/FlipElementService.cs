using Flip.Notifications;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace Flip.Services;

internal class FlipElementService(
    IElementService elementService,
    IIdKeyMap idKeyMap,
    IContentTypeService contentTypeService,
    IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
    IEventAggregator eventAggregator,
    IEventMessagesFactory eventMessagesFactory,
    IAuditService auditService)
    : FlipService<IElement>(
        idKeyMap,
        contentTypeService,
        backOfficeSecurityAccessor,
        eventAggregator,
        eventMessagesFactory,
        auditService)
{
    protected override IPublishableContentService<IElement> EntityService { get; } = elementService;

    protected override UmbracoObjectTypes EntityObjectType => UmbracoObjectTypes.Element;

    protected override CancelableObjectNotification<IElement> CreateChangingNotification(IElement entity, EventMessages messages) =>
        new ElementTypeChangingNotification(entity, messages);

    protected override ObjectNotification<IElement> CreateChangedNotification(IElement entity, EventMessages messages) =>
        new ElementTypeChangedNotification(entity, messages);
}
