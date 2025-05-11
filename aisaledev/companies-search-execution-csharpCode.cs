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
        var companySearchGuid = Parameters["parameter1"];
        var searchDepth = Convert.ToInt32(Parameters["parameter2"]);

        var searchItemResult = ExecuteAgent("companies-search-get", new List<string> { companySearchGuid });
        var companySearchItem = JsonConvert.DeserializeObject<CompanySearchItem>(searchItemResult);
        var companySearchData = JsonConvert.DeserializeObject<CompanySearchData>(companySearchItem.company_search_data);
        var searchQueriesCount = companySearchData.SearchQueries.Count;
        var n = 0;
        foreach (var searchQuery in companySearchData.SearchQueries)
        {
            n++;
            companySearchItem.last_message = $"[{n}/{searchQueriesCount}] Searching '{searchQuery}'";
            Save(companySearchItem, companySearchData, ExecuteAgent);
            var searchResultsJson = ExecuteAgent("companies-search-execution-google", new List<string> { searchQuery });
            var searchResults = JsonConvert.DeserializeObject<List<SearchResult>>(searchResultsJson);
            foreach (var searchResult in searchResults)
            {
                var regEx = GetUrlRegEx(searchResult.url);
                
            }
        }
        return searchItemResult;
    }

    private void Save(CompanySearchItem companySearchItem, CompanySearchData companySearchData, Func<string, List<string>?, string> ExecuteAgent)
    {
        var updatedSearchDataJson = JsonConvert.SerializeObject(companySearchData);
        companySearchItem.company_search_data = updatedSearchDataJson;
        var updatedSearchItemJson = JsonConvert.SerializeObject(companySearchItem);
        ExecuteAgent("companies-search-set", new List<string> { updatedSearchItemJson });
    }

    private string GetUrlRegEx(string url)
    {
        var uri = new Uri(url);            
        string[] parts = uri.Host.Split('.');
        string baseDomain = parts.Length >= 2
            ? string.Join(".", parts[^2], parts[^1])
            : uri.Host;
        return @$"^https?:\/\/([a-zA-Z0-9\-]+\.)*{Regex.Escape(baseDomain)}(?=\/|:|$)";
    }
}

class SearchResult
{
    public string url { get; set; }
    public string text { get; set; }
}

class CompanySearchItem
{
    public int company_search_id { get; set; }
    public string company_search_guid  { get; set; }
    public DateTime executed_at { get; set; }
    public string executed_by { get; set; }
    public string state { get; set; }
    public string company_search_data  { get; set; }
    public string last_message { get; set; }
}

class CompanySearchData
{
    public string QueryString { get; set; } = "";
    public int? SearchDepth { get; set; }
    public List<string> SearchQueries { get; set; } = new List<string>();
}