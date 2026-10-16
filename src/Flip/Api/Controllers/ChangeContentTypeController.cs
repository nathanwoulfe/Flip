using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;

namespace Flip.Api.Controllers;

public class ChangeContentTypeController(IFlipService flipService, IAuthorizationService authorizationService)
    : FlipControllerBase(flipService, authorizationService)
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("change-type")]
    [ProducesResponseType(typeof(FlipOperationStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangeContentType(ChangeDocumentTypeModel model)
    {
        if (await IsAuthorized(model.Unique) is false)
        {
            return Unauthorized();
        }

        Attempt<FlipOperationStatus> result = await FlipService.TryChangeContentType(model);
        return result.Success ? Ok(result.Result) : FlipOperationStatusResult(result.Result);
    }
}
