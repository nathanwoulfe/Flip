using Flip.Models;
using Umbraco.Cms.Core.Models;

namespace Flip.Services;

public interface IFlipService
{
    bool TryChangeContentType(ChangeDocumentTypeModel model, out string message);

    IEnumerable<IContentType> GetPermittedTypes(int nodeId);

    ChangeDocumentTypeModel GetContentModel(int nodeId);
}
