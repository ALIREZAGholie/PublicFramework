using Webgostar.Framework.Infrastructure.InfrastructureModels.DbModels;

namespace Webgostar.Framework.Infrastructure.InfrastructureIServices
{
    public interface ILoggingContext
    {
        void AddLog(string entityName, OperationLogType operation, object data);
        Task FlushLogsAsync();
        void FlushLogs();
    }
}
