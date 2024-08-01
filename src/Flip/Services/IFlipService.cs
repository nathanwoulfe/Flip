using Flip.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;

namespace Flip.Services;

public interface IFlipService
{
    Task<Attempt<bool, string>> ChangeDocumentType(ChangeDocumentTypeRequestModel model);

    IEnumerable<IContentType> GetPermittedTypes(Guid unique);

    Task<ContentModelResponseModel> GetContentModel(Guid unique);
}
