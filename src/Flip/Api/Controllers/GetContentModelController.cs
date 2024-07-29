using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flip.Api.Controllers;

public class GetContentModelController : FlipControllerBase
{
    private readonly IFlipService _flipService;

    public GetContentModelController(IFlipService flipService) => _flipService = flipService;

    [HttpGet("model")]
    [ProducesResponseType(typeof(ChangeDocumentTypeResponseModel), StatusCodes.Status200OK)]
    public IActionResult GetContentModel(Guid unique)
    {
        ChangeDocumentTypeResponseModel contentType = _flipService.GetContentModel(unique);

        return Ok(contentType);
    }
}
