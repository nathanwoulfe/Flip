using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flip.Api.Controllers;

public class GetPermittedTypesController(IFlipServiceFactory flipServiceFactory) : FlipControllerBase(flipServiceFactory)
{
    [HttpGet("permitted")]
    [ProducesResponseType(typeof(IEnumerable<ContentTypeModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid unique, [FromQuery] string entityType)
    {
        IEnumerable<ContentTypeModel> permittedTypes = await FlipServiceFactory.Create(entityType).GetPermittedTypes(unique);
        return Ok(permittedTypes);
    }
}
