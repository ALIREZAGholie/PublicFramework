using Webgostar.Framework.Infrastructure.InfrastructureModels.DbModels.Mongo;

namespace Webgostar.Framework.Infrastructure.InfrastructureIServices
{
    public interface ILogService
    {
        Task InsertManyAsync(IEnumerable<LogEntity> logEntries);
        void InsertMany(IEnumerable<LogEntity> logEntries);
    }
}
