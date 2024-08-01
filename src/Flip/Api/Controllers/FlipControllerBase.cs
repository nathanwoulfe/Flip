using Flip.Api.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Authorization;

namespace Flip.Api.Controllers;

[ApiController]
[MapToApi(ApiConstants.ApiName)]
[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
public abstract class FlipControllerBase : ControllerBase
{
}
