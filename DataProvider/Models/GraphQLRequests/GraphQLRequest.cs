using System.Text.Json;
using DataProvider.Models.Interfaces;

namespace DataProvider.Models.GraphQLRequests;

public class UniSwapGraphQLRequest : IGraphQLRequest
{
    public SwapsParameters Swaps { get; set; } = new();

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }

    public string Serialize()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}

public class SwapsParameters
{
    public int First { get; set; } = 100;
    public string OrderDirection { get; set; } = "desc";
    public int Skip { get; set; } = 0;
    public string OrderBy { get; set; } = "timestamp";
    public string SubgraphError { get; set; } = "allow";
    public WhereParameters Where { get; set; } = new WhereParameters();
}

public class WhereParameters
{
    public string Sender { get; set; } = "0x3fC91A3afd70395Cd496C647d5a6CC9D4B2b7FAD";
    public TokenParameters Token0 { get; set; } = new TokenParameters();
    public TokenParameters Token1 { get; set; } = new TokenParameters();
}

public class TokenParameters
{
    public string Symbol { get; set; }
}
