using DataProvider.Models.GraphQLRequests;
using DataProvider.Models.Interfaces;
using Newtonsoft.Json;


namespace DataProvider.Utilities;

public class GraphQL
{
    public static async Task<string> SendGraphQLRequest(string endpoint, IGraphQLRequest query)
    {
        using var client = new HttpClient();
        var jsonRequest = "{" +  query.Serialize() + "}";
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }       
    public static async Task<string> SendGraphQLRequest(string endpoint, string query)
    {
        using var client = new HttpClient();
        var request = new
        {
            query
        };
        var jsonRequest = JsonConvert.SerializeObject(request);
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    } 
}