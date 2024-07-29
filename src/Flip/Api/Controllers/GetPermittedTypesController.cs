using Flip.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;

namespace Flip.Api.Controllers;

public class GetPermittedTypesController : FlipControllerBase
{
    private readonly IFlipService _flipService;
    public GetPermittedTypesController(IFlipService flipService) => _flipService = flipService;

    [HttpGet("permitted-types")]
    [ProducesResponseType(typeof(IEnumerable<PermittedTypeResponseModel>), StatusCodes.Status200OK)]
    public IActionResult GetPermittedTypes(Guid unique)
    {
        IEnumerable<IContentType> permittedTypes = _flipService.GetPermittedTypes(unique);

        IEnumerable<PermittedTypeResponseModel> mapped = permittedTypes.Select(x => new PermittedTypeResponseModel
        {
            Name = x.Name,
            DefaultTemplateId = x.DefaultTemplateId,
        });

        return Ok(mapped);
    }
}

public class PermittedTypeResponseModel
{
    public string? Name { get; set; }

    public int Id { get; set; }

    public int DefaultTemplateId { get; set; }
}
