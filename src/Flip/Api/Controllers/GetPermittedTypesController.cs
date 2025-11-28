using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flip.Api.Controllers;

public class GetPermittedTypesController : FlipControllerBase
{
    public GetPermittedTypesController(IFlipService flipService) : base(flipService)
    {
    }

    [HttpGet("permitted")]
    [ProducesResponseType(typeof(IEnumerable<ContentTypeModel>), StatusCodes.Status200OK)]
    public IActionResult Get(Guid unique)
    {
        IEnumerable<ContentTypeModel> permittedTypes = FlipService.GetPermittedTypes(unique);

        return Ok(permittedTypes);
    }
}
