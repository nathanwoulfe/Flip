using Asp.Versioning;
using Flip.Api.Configuration;
using Flip.Services;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Web.Common.Routing;

namespace Flip.Api.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = ApiConstants.ApiGroupName)]
[ApiVersion("1.0")]
[BackOfficeRoute($"{ApiConstants.RootPath}/v{{version:apiVersion}}")]
[MapToApi(ApiConstants.ApiName)]
public abstract class FlipControllerBase : ControllerBase
{
    protected IFlipService FlipService { get; }

    public FlipControllerBase(IFlipService flipService) => FlipService = flipService;
}
