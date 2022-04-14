using System.Linq;
#if NETCOREAPP
using Umbraco.Cms.Core.Actions;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#else
using Flip.Web.Security;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web.Actions;
using Umbraco.Web.Models.Trees;
using UmbConstants = Umbraco.Core.Constants;
#endif


namespace Flip.Web.Executors
{
    public interface IContentTreeMenuRenderingExecutor
    {
        void CheckAddFlipAction(string treeAlias, string nodeId, MenuItemCollection menu);
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

        public void CheckAddFlipAction(string treeAlias, string nodeId, MenuItemCollection menu)
        {
            if (treeAlias != UmbConstants.Trees.Content) return;

            var currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity.CurrentUser;
            var showMenu = currentUser.Groups.Any(x => x.Alias.InvariantContains(UmbConstants.Security.AdminGroupAlias));

            if (!showMenu && int.TryParse(nodeId, out int id))
            {
                var permissions = _userService.GetPermissions(currentUser, nodeId);
                showMenu = permissions.AssignedPermissions?.Contains(Constants.ActionLetter) ?? false;
            }

            if (!showMenu) return;

            var item = new MenuItem(Constants.Alias, Constants.ActionName)
            {
                Icon = Constants.Icon,
                SeparatorBefore = false,
                OpensDialog = true
            };

            item.AdditionalData.Add("actionView", Constants.ActionView);

            // add the item after copy, or after move, or second last if neither exist
            if (HasAction(new ActionCopy().Alias)) return;
            if (HasAction(new ActionMove().Alias)) return;

            item.SeparatorBefore = true;
            menu.Items.Insert(menu.Items.Count - 1, item);

            bool HasAction(string alias)
            {
                int index = menu.Items.FindIndex(x => x.Alias == alias);

                if (index != -1)
                {
                    menu.Items.Insert(index + 1, item);
                    return true;
                }

                return false;
            }
        }
    }
}
