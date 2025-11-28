using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Mvc;

namespace Flip.Api.Controllers;

public class ChangeContentTypeController : FlipControllerBase
{
    public ChangeContentTypeController(IFlipService flipService) : base(flipService)
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("change-type")]
    public IActionResult ChangeContentType(ChangeDocumentTypeModel model)
    {
        if (!FlipService.TryChangeContentType(model, out string? message))
        {
            return BadRequest(message);
        }

        return Ok(new { });
    }
}
