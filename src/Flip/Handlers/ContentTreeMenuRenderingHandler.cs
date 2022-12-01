using Flip.Executors;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Flip.Handlers;

public sealed class ContentTreeMenuRenderingHandler : INotificationHandler<MenuRenderingNotification>
{
    private readonly IContentTreeMenuRenderingExecutor _contentTreeMenuRenderingExecutor;

    public ContentTreeMenuRenderingHandler(IContentTreeMenuRenderingExecutor contentTreeMenuRenderingExecutor) =>        
        _contentTreeMenuRenderingExecutor = contentTreeMenuRenderingExecutor;
    

    public void Handle(MenuRenderingNotification notification) =>        
        _contentTreeMenuRenderingExecutor.CheckAddFlipAction(notification.TreeAlias, notification.NodeId, notification.Menu);        
}
