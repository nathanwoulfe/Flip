using Flip.Models;
using Umbraco.Cms.Core.Models;

namespace Flip.Services;

public interface IFlipService
{
    Task<bool> TryChangeContentType(ChangeDocumentTypeResponseModel model);

    IEnumerable<IContentType> GetPermittedTypes(Guid unique);

    ChangeDocumentTypeResponseModel GetContentModel(Guid unique);
}
