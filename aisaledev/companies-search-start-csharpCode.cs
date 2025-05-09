using System;
using System.Collections.Generic;

class Agent
{
    public string Run(
        Dictionary<string, string> Parameters,
        AiCoreApi.Common.RequestAccessor RequestAccessor,
        AiCoreApi.Common.ResponseAccessor ResponseAccessor,
        Func<string, List<string>?, string> ExecuteAgent,
        Func<string, string> GetCacheValue,
        Func<string, string, int, string> SetCacheValue)
    {
        var companySearchGuid = Parameters["parameter1"]; 
        var queryString = Parameters["parameter2"]; 
        var user = RequestAccessor.Login;
        

        var queriesJson = ExecuteAgent("companies-search-queries-prompt", new List<string> { queryString });
        var searchQueries = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(queriesJson)["queries"];
        var companySearchData = new CompanySearchData {
            SearchQueries = searchQueries,
        };

        string sqlInsertQuery = $@"
            INSERT INTO public.company_search (company_search_guid, executed_by, state, company_search_data, last_message)
            VALUES ('{companySearchGuid}', '{user}', 'new', '{{}}', 'Starting..')";
        
        var sqlResult = ExecuteAgent("sql-execute", new List<string> { sqlInsertQuery });
        
        return ExecuteAgent("companies-search-get", new List<string> { companySearchGuid });
    }
}

class CompanySearchData
{
    public string QueryString { get; set; } = "";
    public List<string> SearchQueries { get; set; } = new List<string>();
}