using DataProvider.Models;
using Mediator;

namespace DataProvider.Services.Queries;

public class UniSwapRawQuery : IRequest<RequestResult<List<Swap>>>
{
    public UniSwapRawQuery(int count)
    {
        Count = count;
    }

    public int Count { get; set; }
}