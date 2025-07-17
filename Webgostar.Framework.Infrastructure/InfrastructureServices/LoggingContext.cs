using MongoDB.Bson;
using Webgostar.Framework.Base.IBaseServices;
using Webgostar.Framework.Infrastructure.InfrastructureIServices;
using Webgostar.Framework.Infrastructure.InfrastructureModels.DbModels;
using Webgostar.Framework.Infrastructure.InfrastructureModels.DbModels.Mongo;

namespace Webgostar.Framework.Infrastructure.InfrastructureServices
{
    public class LoggingContext(ILogService logService, IAuthService authService) : ILoggingContext
    {
        private readonly List<(string EntityName, OperationLogType Operation, object Data, string UserId)> _pendingLogs = new();
        private const int batchSize = 1000;

        public void AddLog(string entityName, OperationLogType operation, object data)
        {
            _pendingLogs.Add((entityName, operation, data, authService.GetUserId()));
        }

        public async Task FlushLogsAsync()
        {
            try
            {
                if (_pendingLogs.Any())
                {
                    foreach (var batch in _pendingLogs.Chunk(batchSize))
                    {
                        var logEntries = batch.Select(log => new LogEntity
                        {
                            EntityName = log.EntityName,
                            LogType = log.Operation,
                            UserId = log.UserId,
                            ActionDate = DateTime.UtcNow,
                            EntityData = ToBsonDocument(log.Data)
                        });

                        //await logService.InsertManyAsync(logEntries);
                    }
                    _pendingLogs.Clear();
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void FlushLogs()
        {
            try
            {
                if (!_pendingLogs.Any()) return;

                foreach (var batch in _pendingLogs.Chunk(batchSize))
                {
                    var logEntries = batch.Select(log => new LogEntity
                    {
                        EntityName = log.EntityName,
                        LogType = log.Operation,
                        UserId = log.UserId,
                        ActionDate = DateTime.UtcNow,
                        EntityData = ToBsonDocument(log.Data)
                    });

                    //logService.InsertMany(logEntries);
                }
                _pendingLogs.Clear();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private BsonDocument ToBsonDocument(object data)
        {
            var document = new BsonDocument();
            if (data == null) return document;

            if (data is IEnumerable<object> collection)
            {
                var array = new BsonArray();
                foreach (var item in collection)
                {
                    array.Add(ToBsonDocument(item));
                }
                return new BsonDocument("Items", array);
            }

            var type = data.GetType();
            var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(p => p.Name != "DomainEvents");
            
            foreach (var prop in properties)
            {
                var value = prop.GetValue(data);
                if (value != null)
                {
                    document.Add(prop.Name, BsonValue.Create(value));
                }
            }

            return document;
        }
    }
}
