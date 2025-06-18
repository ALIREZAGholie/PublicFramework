using MediatR;
using Webgostar.Framework.Base.BaseModels;

namespace Webgostar.Framework.Application.QueryCommandTools
{
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, OperationResult>
        where TCommand : ICommand;

    public interface ICommandHandler<in TCommand, TResponseData> : IRequestHandler<TCommand, OperationResult<TResponseData>>
        where TCommand : ICommand<TResponseData>;
}