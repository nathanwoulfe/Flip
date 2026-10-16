using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flip.Api.Controllers;

public class GetPermittedTypesController(IFlipServiceFactory flipServiceFactory, IAuthorizationService authorizationService)
    : FlipControllerBase(flipServiceFactory, authorizationService)
{
    [HttpGet("permitted")]
    [ProducesResponseType(typeof(IEnumerable<ContentTypeModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid unique, [FromQuery] string entityType)
    {
        if (await IsAuthorized(unique, entityType) is false)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        IEnumerable<ContentTypeModel> permittedTypes = await FlipServiceFactory.Create(entityType).GetPermittedTypes(unique);
        return Ok(permittedTypes);
    }
}
