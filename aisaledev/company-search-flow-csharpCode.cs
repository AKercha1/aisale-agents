using System; 
using System.Collections.Generic;
using Newtonsoft.Json;

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
                
                ResponseAccessor.AddDebugMessage("Agent", "Debug", searchResult.Url);
            }
        }
/*
        // Step 4: Prepare to insert results into the database
        for (int i = 0; i < queries.Count; i++)
        {
            // Count the number of results obtained
            int resultCount = searchResults[i].Length; // Assuming the result is a string; adjust as needed

            // Construct SQL query to insert into the database
            string sqlQuery = $@"
            INSERT INTO public.search_query_log (search_engine, query_text, search_count, result_count, result)
            VALUES ('Google', '{queries[i]}', {resultCount}, {resultCount}, '{JsonConvert.SerializeObject(searchResults[i])}');
            ";

            // Execute the SQL query
            ExecuteAgent("sql-execute", new List<string> { sqlQuery });
        }
*/
        // Final result is an empty string as per the requirement
        return "";
    }
}

class SearchResult
{
    public string Url { get; set; }
    public string Text { get; set; }
}