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
        string companySearchGuid = Parameters["parameter1"];
        string queryString = Parameters["parameter2"];

        var searchItemResult = ExecuteAgent("companies-search-get", new List<string> { companySearchGuid });
        var companySearchItem = JsonConvert.DeserializeObject<CompanySearchItem>(searchItemResult);
        var companySearchData = JsonConvert.DeserializeObject<CompanySearchData>(companySearchItem.company_search_data);
        if (companySearchData.SearchQueries != null)
        {
            companySearchData.SearchQueries.Remove(queryString);
        }
        var updatedSearchDataJson = JsonConvert.SerializeObject(companySearchData);
        companySearchItem.company_search_data = updatedSearchDataJson;
        var updatedSearchItemJson = JsonConvert.SerializeObject(companySearchItem);
        ExecuteAgent("companies-search-set", new List<string> { updatedSearchItemJson });
        return updatedSearchItemJson;
    }
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