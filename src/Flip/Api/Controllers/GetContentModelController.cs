using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flip.Api.Controllers;

public class GetContentModelController : FlipControllerBase
{
    public GetContentModelController(IFlipService flipService) : base(flipService)
    {
    }

    [HttpGet("content-model")]
    [ProducesResponseType(typeof(ChangeDocumentTypeModel), StatusCodes.Status200OK)]
    public IActionResult Get(Guid unique)
    {
        ChangeDocumentTypeModel? contentType = FlipService.GetContentModel(unique);

        return Ok(contentType);
    }
}
