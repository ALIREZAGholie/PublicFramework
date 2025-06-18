using Webgostar.Framework.Base.BaseModels;

namespace Webgostar.Framework.Base.IBaseServices
{
    public interface IBaseRepository<TEntity> : IBasePublisherRepository<TEntity>, IBaseReadRepository<TEntity> where TEntity : BaseEntity
    {

    }
}