using Flip.Api.Controllers.Type;
using Flip.Models;
using Flip.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Flip.Api.Controllers;

public class GetPermittedTypesController : TypeControllerBase
{
    private readonly IFlipService _flipService;
    private readonly IDataTypeService _dataTypeService;

    public GetPermittedTypesController(IFlipService flipService, IDataTypeService dataTypeService)
    {
        _flipService = flipService;
        _dataTypeService = dataTypeService;
    }

    [HttpGet("permitted")]
    [ProducesResponseType(typeof(IEnumerable<PermittedTypeResponseModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermittedTypes(Guid unique)
    {
        IEnumerable<IContentType> permittedTypes = _flipService.GetPermittedTypes(unique);

        IEnumerable<PermittedTypeResponseModel> mapped = await Task.WhenAll(permittedTypes.Select(async permittedType =>
        {
            IEnumerable<IPropertyType> compositionPropertyTypes = permittedType.ContentTypeComposition
                .SelectMany(composition =>
                    composition.PropertyGroups
                        .Where(propertyGroup => propertyGroup.PropertyTypes != null)
                        .SelectMany(propertyGroup => propertyGroup.PropertyTypes!));

            IEnumerable<IPropertyType> propertyTypes = permittedType.PropertyGroups
                .Where(propertyGroup => propertyGroup != null)
                .SelectMany(propertyGroup => propertyGroup.PropertyTypes!);

            IEnumerable<IPropertyType> allPropertyTypes = compositionPropertyTypes.Concat(propertyTypes);
            IEnumerable<IDataType> dataTypes = await _dataTypeService.GetAllAsync(allPropertyTypes.Select(p => p.DataTypeKey).ToArray());

            return new PermittedTypeResponseModel
            {
                Name = permittedType.Name ?? permittedType.Alias,
                Unique = permittedType.Key,
                Alias = permittedType.Alias,
                Icon = permittedType.Icon,
                DefaultTemplateId = permittedType.DefaultTemplateId,
                AllowedTemplates = permittedType.AllowedTemplates?
                    .Select(x => new TemplateResponseModel { Name = x.Name, Id = x.Id }) ?? [],
                PropertyTypes = allPropertyTypes
                    .Select(p => new PropertyTypeResponseModel
                    {
                        Name = p.Name,
                        Alias = p.Alias,
                        DataTypeKey = p.DataTypeKey,
                        PropertyEditorAlias = dataTypes.First(x => x.Key == p.DataTypeKey)?.EditorUiAlias,
                    }),
            };
        }));

        return Ok(mapped);
    }
}
