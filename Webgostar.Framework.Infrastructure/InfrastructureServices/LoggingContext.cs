using MongoDB.Bson;
using Webgostar.Framework.Base.IBaseServices;
using Webgostar.Framework.Infrastructure.InfrastructureIServices;
using Webgostar.Framework.Infrastructure.InfrastructureModels.DbModels;
using Webgostar.Framework.Infrastructure.InfrastructureModels.DbModels.Mongo;

namespace Webgostar.Framework.Infrastructure.InfrastructureServices
{
    public class LoggingContext : ILoggingContext
    {
        private readonly ILogService _logService;
        private readonly IAuthService _authService;
        private readonly List<(string EntityName, OperationLogType Operation, object Data, long UserId)> _pendingLogs;
        private const int batchSize = 1000;

        public LoggingContext(ILogService logService, IAuthService authService)
        {
            _logService = logService;
            _authService = authService;
            _pendingLogs = new List<(string, OperationLogType, object, long)>();
        }

        public void AddLog(string entityName, OperationLogType operation, object data)
        {
            _pendingLogs.Add((entityName, operation, data, _authService.GetUserId()));
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

                        await _logService.InsertManyAsync(logEntries);
                    }
                    _pendingLogs.Clear();
                }
            }
            catch (Exception)
            {
                
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

                    _logService.InsertMany(logEntries);
                }
                _pendingLogs.Clear();
            }
            catch (Exception)
            {
                
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
