#if NETCOREAPP
using Flip.Web.Executors;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;

namespace Flip.Web.Handlers
{
    public class ContentTreeMenuRenderingHandler : INotificationHandler<MenuRenderingNotification>
    {
        private readonly IContentTreeMenuRenderingExecutor _contentTreeMenuRenderingExecutor;

        public ContentTreeMenuRenderingHandler(IContentTreeMenuRenderingExecutor contentTreeMenuRenderingExecutor) =>        
            _contentTreeMenuRenderingExecutor = contentTreeMenuRenderingExecutor;
        

        public void Handle(MenuRenderingNotification notification) =>        
            _contentTreeMenuRenderingExecutor.CheckAddFlipAction(notification.TreeAlias, notification.NodeId, notification.Menu);        
    }
}
#endif
