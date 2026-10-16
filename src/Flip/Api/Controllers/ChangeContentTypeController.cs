using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;

namespace Flip.Api.Controllers;

public class ChangeContentTypeController(IFlipServiceFactory flipServiceFactory, IAuthorizationService authorizationService)
    : FlipControllerBase(flipServiceFactory, authorizationService)
{
    [HttpPost("change-type")]
    [ProducesResponseType(typeof(FlipOperationStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangeContentType(ChangeEntityTypeModel model, [FromQuery] string entityType)
    {
        if (await IsAuthorized(model.Unique, entityType) is false)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        Attempt<FlipOperationStatus> result = await FlipServiceFactory.Create(entityType).TryChangeContentType(model);
        return result.Success ? Ok(result.Result) : FlipOperationStatusResult(result.Result);
    }
}
