using Flip.Api.Controllers.Type;
using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;

namespace Flip.Api.Controllers;

public class ChangeDocumentTypeController : TypeControllerBase
{
    private readonly IFlipService _flipService;

    public ChangeDocumentTypeController(IFlipService flipService) => _flipService = flipService;

    [HttpPost("change")]
    [ProducesResponseType(typeof(ChangeDocumentTypeResultModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangeDocumentType(ChangeDocumentTypeRequestModel model)
    {
        Attempt<bool, string> result = await _flipService.ChangeDocumentType(model);
        return Ok(new ChangeDocumentTypeResultModel { Message = result.Status, Success = result.Success });
    }
}
