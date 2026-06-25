using Flip.Models;
using Umbraco.Cms.Core.Models;

namespace Flip.Services;

public interface IFlipService
{
    bool TryChangeContentType(ChangeEntityTypeModel model, out string? message);

    Task<IEnumerable<ContentTypeModel>> GetPermittedTypes(Guid unique);

    ChangeEntityTypeModel? GetContentModel(Guid unique);
}

public interface IFlipService<TEntity> : IFlipService
    where TEntity : class, IPublishableContentBase;

public interface IFlipServiceFactory
{
    IFlipService Create(string entityType);
}
