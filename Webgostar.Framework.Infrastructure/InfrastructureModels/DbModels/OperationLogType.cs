using System.ComponentModel;

namespace Webgostar.Framework.Infrastructure.InfrastructureModels.DbModels
{
    public enum OperationLogType
    {
        [Description("")]
        Add = 1,
        Update = 2,
        Delete = 3,
        Recovery = 4
    }
}
