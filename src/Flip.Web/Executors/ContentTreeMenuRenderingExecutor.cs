using System;

namespace Flip.Web.Executors
{
    public interface IContentTreeMenuRenderingExecutor
    {
        void CheckAddFlipAction(string treeAlias, int nodeId);
    }

    public class ContentTreeMenuRenderingExecutor : IContentTreeMenuRenderingExecutor
    {
        public void CheckAddFlipAction(string treeAlias, int nodeId)
        {
            throw new NotImplementedException();
        }
    }
}
