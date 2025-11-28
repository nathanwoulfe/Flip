using Flip.Models;

namespace Flip.Services;

public interface IFlipService
{
    bool TryChangeContentType(ChangeDocumentTypeModel model, out string? message);

    IEnumerable<ContentTypeModel> GetPermittedTypes(Guid unique);

    ChangeDocumentTypeModel? GetContentModel(Guid unique);
}
