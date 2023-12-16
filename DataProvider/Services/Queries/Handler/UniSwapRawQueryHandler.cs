using DataProvider.Models;
using Mediator;

namespace DataProvider.Services.Queries.Handler;

public class UniSwapRawQueryHandler : IRequestHandler<UniSwapRawQuery, RequestResult<List<Swap>>>
{
    private readonly HttpClient _client;

    public UniSwapRawQueryHandler(HttpClient client)
    {
        _client = client;
    }

    public async ValueTask<RequestResult<List<Swap>>> Handle(UniSwapRawQuery request, CancellationToken cancellationToken)
    {

        try
        {
            var result = await Utilities.GraphQL.SendGraphQLRequest(Consts.Consts.UNISWAP_V3_GRAPHQL_URL,"""
                {
                  swaps(
                    first: 100
                    orderDirection: desc
                    skip: 0
                    orderBy: timestamp
                    subgraphError: allow
                    where: { token0_: {symbol: "WETH"}, token1_: {symbol: "USDT"}, transaction_: {}}
                  ) {
                    amount0
                    sqrtPriceX96
                    timestamp
                    amount1
                    token1 {
                      symbol
                    }
                    token0 {
                      symbol
                    }
                    tick
                    amountUSD
                    recipient
                    origin
                    sender
                    transaction {
                      gasPrice
                      gasUsed
                      id
                      timestamp
                      blockNumber
                    }
                  }
                }
                """.Replace("100", request.Count.ToString()));

            var final = System.Text.Json.JsonSerializer.Deserialize<UniSwapCommittedSwapsDto>(result);
            return final!.Data.Swaps;
        }
        catch (Exception e)
        {
            return e;
        }
        
    }
}