using Asp.Versioning;
using Flip.Api.Configuration;
using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Common.Builders;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security.Authorization;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;
using Umbraco.Extensions;

namespace Flip.Api.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = ApiConstants.ApiGroupName)]
[ApiVersion("1.0")]
[BackOfficeRoute($"{ApiConstants.RootPath}/v{{version:apiVersion}}")]
[MapToApi(ApiConstants.ApiName)]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
public abstract class FlipControllerBase(IFlipServiceFactory flipServiceFactory, IAuthorizationService authorizationService) : ControllerBase
{
    private readonly IAuthorizationService _authorizationService = authorizationService;
    protected IFlipServiceFactory FlipServiceFactory { get; } = flipServiceFactory;
    protected const string ChangeDocumentTypePermission = "Flip.ChangeDocumentType";
    protected const string ChangeElementTypePermission = "Flip.ChangeElementType";

    protected async Task<bool> IsAuthorized(Guid unique, string entityType)
    {
        // authorize against the same entity type the factory resolves, so the permission checked matches the service used
        AuthorizationResult authorizationResult = FlipServiceFactory.Create(entityType) is IFlipService<IElement>
            ? await _authorizationService.AuthorizeResourceAsync(
                User,
                ElementPermissionResource.WithKeys(ChangeElementTypePermission, unique),
                AuthorizationPolicies.ElementPermissionByResource)
            : await _authorizationService.AuthorizeResourceAsync(
                User,
                ContentPermissionResource.WithKeys(ChangeDocumentTypePermission, unique),
                AuthorizationPolicies.ContentPermissionByResource);

        return authorizationResult.Succeeded;
    }

    protected IActionResult FlipOperationStatusResult(FlipOperationStatus status) =>
        status switch
        {
            FlipOperationStatus.CancelledByNotification => CancelledByNotification(),
            FlipOperationStatus.CurrentAndNewTypesMatch => BadRequest(new ProblemDetailsBuilder()
                .WithTitle("Current type and target type are the same")
                .Build()),
            FlipOperationStatus.ContentNotFound => NotFound(new ProblemDetailsBuilder()
                .WithTitle("The requested content could not be found")
                .Build()),
            FlipOperationStatus.ContentTypeNotFound => NotFound(new ProblemDetailsBuilder()
                .WithTitle("The requested content type could not be found")
                .Build()),
            FlipOperationStatus.UserNotFound => NotFound(new ProblemDetailsBuilder()
                .WithTitle("No Backoffice user found")
                .Build()),
            FlipOperationStatus.Unknown => Unknown(),
            _ => Default(),
        };

    protected ObjectResult Unknown() =>
        StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetailsBuilder()
            .WithTitle("Unknown error. Please see the log for more details.")
            .Build());

    protected ObjectResult Default() =>
        StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetailsBuilder()
            .WithTitle("Unknown operation status.")
            .Build());

    protected BadRequestObjectResult CancelledByNotification() =>
        BadRequest(new ProblemDetailsBuilder()
            .WithTitle("Cancelled by notification")
            .WithDetail("A notification handler prevented the operation.")
            .Build());
}
