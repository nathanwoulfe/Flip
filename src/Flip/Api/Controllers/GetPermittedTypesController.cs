using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flip.Api.Controllers;

public class GetPermittedTypesController(IFlipService flipService, IAuthorizationService authorizationService)
    : FlipControllerBase(flipService, authorizationService)
{
    [HttpGet("permitted")]
    [ProducesResponseType(typeof(IEnumerable<ContentTypeModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get(Guid unique)
    {
        if (await IsAuthorized(unique) is false)
        {
            return Unauthorized();
        }

        IEnumerable<ContentTypeModel> permittedTypes = FlipService.GetPermittedTypes(unique);
        return Ok(permittedTypes);
    }
}
