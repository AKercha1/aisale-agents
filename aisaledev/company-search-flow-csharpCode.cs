using System; 
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

class Agent 
{
    public string Run(
        Dictionary<string, string> Parameters,
        AiCoreApi.Common.RequestAccessor RequestAccessor,
        AiCoreApi.Common.ResponseAccessor ResponseAccessor,
        Func<string, List<string>?, string> ExecuteAgent,
        Func<string, string> GetCacheValue,
        Func<string, string, int, string> SetCacheValue,
        Microsoft.Extensions.Logging.ILogger<AiCoreApi.SemanticKernel.Agents.CsharpCodeAgent> Logger)
    {
        var area = Parameters["parameter1"];
        var queriesJson = ExecuteAgent("company-search-generate-prompt", new List<string> { "1", "USA", area, "50-500", "Google" });
        var queries = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(queriesJson)["queries"];
        foreach (var query in queries)
        {
            var googleSearchToolResult = ExecuteAgent("google-search-tool", new List<string> { query, "10", "0" });
            var searchResults = JsonConvert.DeserializeObject<List<SearchResult>>(googleSearchToolResult);
            foreach (var searchResult in searchResults)
            {
                var uri = new Uri(searchResult.Url);            
                string[] parts = uri.Host.Split('.');
                string baseDomain = parts.Length >= 2
                    ? string.Join(".", parts[^2], parts[^1])
                    : uri.Host;
                string domainPattern = @$"^https?:\/\/([a-zA-Z0-9\-]+\.)*{Regex.Escape(baseDomain)}(?=\/|:|$)";
                ResponseAccessor.AddDebugMessage("Agent", "Debug", searchResult.Url + " " + domainPattern);

                var crawlerToolResult = ExecuteAgent("crawler-tool", new List<string> { searchResult.Url, domainPattern });
                ResponseAccessor.AddDebugMessage("Agent", "Debug", crawlerToolResult);
                return "";
            }
        }
        return "";
    }
}

class SearchResult
{
    public string Url { get; set; }
    public string Text { get; set; }
}