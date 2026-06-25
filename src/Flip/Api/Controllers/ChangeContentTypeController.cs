using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flip.Api.Controllers;

public class ChangeContentTypeController(IFlipServiceFactory flipServiceFactory) : FlipControllerBase(flipServiceFactory)
{
    [HttpPost("change-type")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult ChangeContentType(ChangeEntityTypeModel model, [FromQuery] string entityType)
    {
        if (!FlipServiceFactory.Create(entityType).TryChangeContentType(model, out string? message))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Detail = message,
            });
        }

        return Ok();
    }
}
