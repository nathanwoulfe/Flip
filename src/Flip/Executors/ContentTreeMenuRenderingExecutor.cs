using Umbraco.Cms.Core.Actions;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace Flip.Executors;

public interface IContentTreeMenuRenderingExecutor
{
    void CheckAddFlipAction(string treeAlias, string nodeId, MenuItemCollection menu);
}

internal sealed class ContentTreeMenuRenderingExecutor : IContentTreeMenuRenderingExecutor
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
        if (treeAlias != UmbConstants.Trees.Content)
        {
            return;
        }

        IUser? currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;

        if (currentUser is null)
        {
            return;
        }

        bool showMenu = false;

        if (int.TryParse(nodeId, out int id))
        {
            EntityPermission? permissions = _userService.GetPermissions(currentUser, nodeId);
            showMenu = permissions?.AssignedPermissions?.Contains(Constants.ActionLetter) ?? false;
        }

        if (!showMenu)
        {
            return;
        }

        MenuItem item = new(Constants.Alias, Constants.ActionName)
        {
            Icon = Constants.Icon,
            SeparatorBefore = false,
            OpensDialog = true,
        };

        item.AdditionalData.Add("actionView", Constants.ActionView);

        // add the item after copy, or after move, or second last if neither exist
        if (HasAction(new ActionCopy().Alias))
        {
            return;
        }

        if (HasAction(new ActionMove().Alias))
        {
            return;
        }

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
