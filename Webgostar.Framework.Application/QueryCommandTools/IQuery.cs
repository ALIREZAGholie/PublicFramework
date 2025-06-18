using MediatR;
using Webgostar.Framework.Application.ApplicationModels;
using Webgostar.Framework.Base.BaseModels.GridData;

namespace Webgostar.Framework.Application.QueryCommandTools;

public interface IQuery<out TResponse> : IRequest<TResponse>;

public class QueryGrid<TResponse> : IRequest<GridData<TResponse>> where TResponse : BaseDto
{
    public QueryGrid(BaseGrid baseGrid)
    {
        BaseGrid = baseGrid;
    }

    public BaseGrid BaseGrid { get; private set; }
}