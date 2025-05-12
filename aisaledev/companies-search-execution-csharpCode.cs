using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Linq;

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
        var searchLimit = Convert.ToInt32(Parameters["parameter3"]);

        var searchItemResult = ExecuteAgent("companies-search-get", new List<string> { companySearchGuid });
        var companySearchItem = JsonConvert.DeserializeObject<CompanySearchItem>(searchItemResult);
        var companySearchData = JsonConvert.DeserializeObject<CompanySearchData>(companySearchItem.company_search_data);
        companySearchData.SearchDepth = searchDepth;
        companySearchData.SearchLimit = searchLimit;
        companySearchItem.state = "in progress";
        var searchQueriesCount = companySearchData.SearchQueries.Count;
        var searchQueriesN = 0;
        var companiesCount = 1;
        foreach (var searchQuery in companySearchData.SearchQueries)
        {
            searchQueriesN++;            
            companySearchItem.log = $"{companySearchItem.last_message}{Environment.NewLine}" + companySearchItem.log;
            companySearchItem.last_message = $"[{searchQueriesN}/{searchQueriesCount}] Searching {searchQuery}";
            Save(companySearchItem, companySearchData, ExecuteAgent);
            var searchResultsJson = ExecuteAgent("companies-search-execution-google", new List<string> { searchQuery });
            var searchResults = JsonConvert.DeserializeObject<List<SearchResult>>(searchResultsJson);
            var searchResultsN = 0;
            foreach (var searchResult in searchResults)
            {
                searchResultsN++;
                companySearchItem.log = $"{companySearchItem.last_message}{Environment.NewLine}" + companySearchItem.log;
                companySearchItem.last_message = $"[{searchQueriesN}/{searchQueriesCount}][{searchResultsN}/{searchResults.Count}] Crawling {searchResult.url} ({searchResult.text})";
                Save(companySearchItem, companySearchData, ExecuteAgent);
                var urlRegEx = GetUrlRegEx(searchResult.url);
                var crawlerResultsJson = ExecuteAgent("companies-search-execution-crawler", new List<string> { searchResult.url, urlRegEx, searchDepth.ToString(), searchLimit.ToString() });
                var crawlerResults = JsonConvert.DeserializeObject<List<CrawlerResult>>(crawlerResultsJson);
                var crawlerResultsN = 0;
                foreach (var crawlerResult in crawlerResults)
                {
                    crawlerResultsN++;
                    companySearchItem.log = $"{companySearchItem.last_message}{Environment.NewLine}" + companySearchItem.log;
                    companySearchItem.last_message = $"[{searchQueriesN}/{searchQueriesCount}][{searchResultsN}/{searchResults.Count}][{crawlerResultsN}/{crawlerResults.Count}] "
                        + $"Extracting Companies {crawlerResult.url}";
                    Save(companySearchItem, companySearchData, ExecuteAgent);
                    var companiesResultsJson = ExecuteAgent("companies-search-execution-crawler-prompt", new List<string> { crawlerResult.url, companySearchData.QueryString });
                    var companiesResults = JsonConvert.DeserializeObject<CompanySearchData>(companiesResultsJson);
                    foreach(var company in companiesResults.Companies)
                    {
                        // Check if the company already exists in the list
                        if(companySearchData.Companies.FirstOrDefault(x => x.name == company.name) != null)
                            continue;
                        company.id = companiesCount;
                        companiesCount++;
                        companySearchData.Companies.Add(company);
                        companySearchItem.log = $"- Company {company.name} added.{Environment.NewLine}" + companySearchItem.log;
                    }
                    Save(companySearchItem, companySearchData, ExecuteAgent);
                }                
            }
        }
        companySearchItem.state = "done";
        
        companySearchItem.log = $"{companySearchItem.last_message}{Environment.NewLine}" + companySearchItem.log;
        companySearchItem.last_message = $"Finished.";
        Save(companySearchItem, companySearchData, ExecuteAgent);
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

class CrawlerResult
{
    public string url { get; set; }
    public string text { get; set; }
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
    public string log { get; set; }
}

class CompanySearchData
{
    public string QueryString { get; set; } = "";
    public int? SearchDepth { get; set; }
    public int? SearchLimit { get; set; }
    public List<string> SearchQueries { get; set; } = new List<string>();
    public List<CompanyDetails> Companies { get; set; } = new List<CompanyDetails>();
}

class CompanyDetails
{
    public int id { get; set; } = 1;
    public string name { get; set; } = "";
    public string url { get; set; } = "";
    public string details { get; set; } = "";
}