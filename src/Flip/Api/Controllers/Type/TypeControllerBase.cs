using Flip.Api.Configuration;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Routing;

namespace Flip.Api.Controllers.Type;

[ApiExplorerSettings(GroupName = "Type")]
[BackOfficeRoute($"{ApiConstants.RootPath}/v{{version:apiVersion}}/type")]
public abstract class TypeControllerBase : FlipControllerBase
{
}
