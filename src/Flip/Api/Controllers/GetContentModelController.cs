using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flip.Api.Controllers;

public class GetContentModelController(IFlipServiceFactory flipServiceFactory) : FlipControllerBase(flipServiceFactory)
{
    [HttpGet("content-model")]
    [ProducesResponseType(typeof(ChangeEntityTypeModel), StatusCodes.Status200OK)]
    public IActionResult Get(Guid unique, [FromQuery] string entityType)
    {
        ChangeEntityTypeModel? contentType = FlipServiceFactory.Create(entityType).GetContentModel(unique);
        return Ok(contentType);
    }
}
