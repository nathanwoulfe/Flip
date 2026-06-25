using Umbraco.Cms.Core.Models;

namespace Flip.Services;

internal sealed class FlipServiceFactory(
    IFlipService<IContent> documentService,
    IFlipService<IElement> elementService) : IFlipServiceFactory
{
    private const string ElementEntityType = "element";

    public IFlipService Create(string entityType) =>
        string.Equals(entityType, ElementEntityType, StringComparison.OrdinalIgnoreCase)
            ? elementService
            : documentService;
}
