#if NETFRAMEWORK
using Flip.Web.Executors;
using Umbraco.Core.Composing;
using Umbraco.Web.Trees;

namespace Flip.Web.Components
{
    public class ContentTreeMenuRenderingComponent : IComponent
    {
        private readonly IContentTreeMenuRenderingExecutor _contentTreeMenuRenderingExecutor;

        public ContentTreeMenuRenderingComponent(IContentTreeMenuRenderingExecutor contentTreeMenuRenderingExecutor) =>        
            _contentTreeMenuRenderingExecutor = contentTreeMenuRenderingExecutor;        

        public void Initialize() => ContentTreeController.MenuRendering += ContentTreeController_MenuRendering;        

        public void Terminate() => ContentTreeController.MenuRendering -= ContentTreeController_MenuRendering;

        private void ContentTreeController_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e) =>        
            _contentTreeMenuRenderingExecutor.CheckAddFlipAction(sender.TreeAlias, e.NodeId, e.Menu);
    }
}
#endif