using MediatR;
using Webgostar.Framework.Base.BaseModels;

namespace Webgostar.Framework.Application.QueryCommandTools
{
    public interface ICommand : IRequest<OperationResult>;

    public interface ICommand<TData> : IRequest<OperationResult<TData>>;
}