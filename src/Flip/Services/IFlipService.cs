using Flip.Models;
using Umbraco.Cms.Core;

namespace Flip.Services;

public interface IFlipService
{
    public Task<Attempt<FlipOperationStatus>> TryChangeContentType(ChangeDocumentTypeModel model);

    public IEnumerable<ContentTypeModel> GetPermittedTypes(Guid unique);

    public ChangeDocumentTypeModel? GetContentModel(Guid unique);
}
