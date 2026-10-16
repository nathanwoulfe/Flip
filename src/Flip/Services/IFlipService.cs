using Flip.Models;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;

namespace Flip.Services;

public interface IFlipService
{
    public Task<Attempt<FlipOperationStatus>> TryChangeContentType(ChangeEntityTypeModel model);

    public Task<IEnumerable<ContentTypeModel>> GetPermittedTypes(Guid unique);

    public ChangeEntityTypeModel? GetContentModel(Guid unique);
}

public interface IFlipService<TEntity> : IFlipService
    where TEntity : class, IPublishableContentBase;

public interface IFlipServiceFactory
{
    public IFlipService Create(string entityType);
}
