using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flip.Api.Controllers;

public class GetContentModelController(IFlipServiceFactory flipServiceFactory, IAuthorizationService authorizationService)
    : FlipControllerBase(flipServiceFactory, authorizationService)
{
    [HttpGet("content-model")]
    [ProducesResponseType(typeof(ChangeEntityTypeModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid unique, [FromQuery] string entityType)
    {
        if (await IsAuthorized(unique, entityType) is false)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        ChangeEntityTypeModel? contentType = FlipServiceFactory.Create(entityType).GetContentModel(unique);
        return Ok(contentType);
    }
}
