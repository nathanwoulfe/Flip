using Umbraco.Cms.Core.Trees;

namespace Flip.Executors;

public interface IContentTreeMenuRenderingExecutor
{
    void CheckAddFlipAction(string treeAlias, string nodeId, MenuItemCollection menu);
}
