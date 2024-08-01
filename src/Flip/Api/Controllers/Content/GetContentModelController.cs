using Flip.Api.Configuration;
using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Routing;

namespace Flip.Api.Controllers;

[ApiExplorerSettings(GroupName = "Content")]
[BackOfficeRoute($"{ApiConstants.RootPath}/v{{version:apiVersion}}/content")]
public class GetContentModelController : FlipControllerBase
{
    private readonly IFlipService _flipService;

    public GetContentModelController(IFlipService flipService) => _flipService = flipService;

    [HttpGet]
    [ProducesResponseType(typeof(ContentModelResponseModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContentModel(Guid unique)
    {
        ContentModelResponseModel contentType = await _flipService.GetContentModel(unique);
        return Ok(contentType);
    }
}
