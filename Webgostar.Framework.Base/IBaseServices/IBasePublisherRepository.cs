using Webgostar.Framework.Base.BaseModels;

namespace Webgostar.Framework.Base.IBaseServices
{
    public interface IBasePublisherRepository<in TEntity> where TEntity : BaseEntity
    {
        Task Add(TEntity entity, CancellationToken cancellationToken = new CancellationToken());
        Task Update(TEntity entity);
        Task Remove(long id, CancellationToken cancellationToken = new CancellationToken());
    }
}
