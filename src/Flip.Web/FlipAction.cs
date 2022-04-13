#if NETCOREAPP
using Umbraco.Cms.Core.Actions;
#else
using Umbraco.Web.Actions;
#endif

namespace Flip.Web
{
    public class FlipAction : IAction
    {
        public char Letter => 'F';

        public bool ShowInNotifier => true;

        public bool CanBePermissionAssigned => true;

        public string Icon => "binoculars";

        public string Alias => "flip";

        public string Category => "structure";
    }
}
