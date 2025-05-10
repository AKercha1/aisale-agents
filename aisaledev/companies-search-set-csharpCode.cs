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
        var companiesSearchJson = Parameters["parameter1"];
        var companiesSearch = JsonConvert.DeserializeObject<CompanySearchItem>(companiesSearchJson);

        var sqlUpdateQuery = $@"
            UPDATE public.company_search
            SET 
                executed_at = '{companiesSearch.executed_at}',
                executed_by = '{companiesSearch.executed_by}',
                state = '{companiesSearch.state}',
                company_search_data = '{JsonConvert.SerializeObject(companiesSearch.company_search_data)}',
                last_message = '{companiesSearch.last_message}'
            WHERE company_search_guid = '{companiesSearch.company_search_guid}'";

        var sqlResult = ExecuteAgent("sql-execute", new List<string> { sqlUpdateQuery });

        return sqlResult;
    }
}

class CompanySearchItem
{
    public int company_search_id { get; set; }
    public string company_search_guid { get; set; }
    public DateTime executed_at { get; set; }
    public string executed_by { get; set; }
    public string state { get; set; }
    public CompanySearchData company_search_data { get; set; }
    public string last_message { get; set; }
}

class CompanySearchData
{
    public string QueryString { get; set; } = "";
    public int? SearchDepth { get; set; }
    public List<string> SearchQueries { get; set; } = new List<string>();
}