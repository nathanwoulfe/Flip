#if NETCOREAPP
using Umbraco.Cms.Core.Actions;
#else
using Umbraco.Web.Actions;
#endif

namespace Flip.Web
{
    public class FlipAction : IAction
    {
        public char Letter => Constants.ActionLetter[0];

        public bool ShowInNotifier => true;

        public bool CanBePermissionAssigned => true;

        public string Icon => Constants.Icon;

        public string Alias => Constants.Alias;

        public string Category => Constants.Category;
    }
}
