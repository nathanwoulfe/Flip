using Flip.Services;
using Flip.Models;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Core.Models;

namespace Flip.Controllers;

[PluginController(Constants.Name)]
public class ApiController : UmbracoAuthorizedApiController
{
    private readonly IFlipService _flipService;

    public ApiController(IFlipService flipService)
    {
        _flipService = flipService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetPermittedTypes(int nodeId)
    {
        IEnumerable<IContentType> permittedTypes = _flipService.GetPermittedTypes(nodeId);

        return Ok(new { 
            permittedTypes,
        });
    }

    [HttpGet]
    public IActionResult GetContentModel(int nodeId)
    {
        ChangeDocumentTypeModel contentType = _flipService.GetContentModel(nodeId);

        return Ok(contentType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult ChangeContentType(ChangeDocumentTypeModel model)
    {
        if (!_flipService.TryChangeContentType(model, out string message))
        {
            return BadRequest(message);
        }

        // TODO => return something here
        return Ok(new { });
    }
}
