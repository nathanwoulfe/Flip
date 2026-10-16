using Flip.Notifications;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using ContentTypeChangedNotification = Flip.Notifications.ContentTypeChangedNotification;

namespace Flip.Services;

internal class FlipDocumentService(
    IContentService contentService,
    IIdKeyMap idKeyMap,
    IContentTypeService contentTypeService,
    IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
    IEventAggregator eventAggregator,
    IEventMessagesFactory eventMessagesFactory,
    IAuditService auditService)
    : FlipService<IContent>(
        idKeyMap,
        contentTypeService,
        backOfficeSecurityAccessor,
        eventAggregator,
        eventMessagesFactory,
        auditService)
{
    protected override IPublishableContentService<IContent> EntityService { get; } = contentService;

    protected override UmbracoObjectTypes EntityObjectType => UmbracoObjectTypes.Document;

    protected override CancelableObjectNotification<IContent> CreateChangingNotification(IContent entity, EventMessages messages) =>
        new ContentTypeChangingNotification(entity, messages);

    protected override ObjectNotification<IContent> CreateChangedNotification(IContent entity, EventMessages messages) =>
        new ContentTypeChangedNotification(entity, messages);
}
