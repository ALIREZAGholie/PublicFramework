using MediatR;
using Webgostar.Framework.Application.ApplicationModels;
using Webgostar.Framework.Base.BaseModels.GridData;

namespace Webgostar.Framework.Application.QueryCommandTools;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;

public interface IQueryGridHandler<in TQuery, TResponse> : IRequestHandler<TQuery, GridData<TResponse>>
    where TResponse : BaseDto
    where TQuery : QueryGrid<TResponse>;