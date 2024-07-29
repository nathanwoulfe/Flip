using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flip.Api.Controllers;

public class ChangeContentTypeController : FlipControllerBase
{
    private readonly IFlipService _flipService;

    public ChangeContentTypeController(IFlipService flipService) => _flipService = flipService;

    [HttpPost("change-type")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangeContentType(ChangeDocumentTypeResponseModel model)
    {
        if (await _flipService.TryChangeContentType(model) is false)
        {
            return BadRequest();
        }

        // TODO => return something here
        return Ok(new { });
    }
}
