using System.Linq;
#if NETCOREAPP
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#else
using Flip.Web.Security;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using UmbConstants = Umbraco.Core.Constants;
#endif


namespace Flip.Web.Executors
{
    public interface IContentTreeMenuRenderingExecutor
    {
        bool CheckAddFlipAction(string treeAlias, string nodeId, out MenuItem menuItem);
    }

    public class ContentTreeMenuRenderingExecutor : IContentTreeMenuRenderingExecutor
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IUserService _userService;

        public ContentTreeMenuRenderingExecutor(
            IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
            IUserService userService)
        {
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            _userService = userService;
        }

        public bool CheckAddFlipAction(string treeAlias, string nodeId, out MenuItem menuItem)
        {
            menuItem = null;

            if (treeAlias != UmbConstants.Trees.Content) return false;

            var currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity.CurrentUser;
            var showMenu = currentUser.Groups.Any(x => x.Alias.InvariantContains(UmbConstants.Security.AdminGroupAlias));

            if (!showMenu && int.TryParse(nodeId, out int id))
            {
                var permissions = _userService.GetPermissions(currentUser, nodeId);
                showMenu = permissions.AssignedPermissions?.Contains(Constants.ActionLetter) ?? false;
            }

            if (!showMenu) return false;

            var item = new MenuItem(Constants.Alias, Constants.ActionName)
            {
                Icon = Constants.Icon,
                SeparatorBefore = true
            };

            item.AdditionalData.Add("actionView", Constants.ActionView);

            return true;
        }
    }
}
