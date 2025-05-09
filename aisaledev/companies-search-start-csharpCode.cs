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
        Func<string, string, int, string> SetCacheValue)
    {
        var companySearchGuid = Parameters["parameter1"]; 
        var queryString = Parameters["parameter2"]; 
        var searchStringCount = Parameters["parameter3"]; 
        var user = RequestAccessor.Login;
        var queriesJson = ExecuteAgent("companies-search-queries-prompt", new List<string> { queryString, searchStringCount });
        var searchQueries = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(queriesJson)["queries"];
        var companySearchData = new CompanySearchData {
            QueryString = queryString,
            SearchQueries = searchQueries,
        };

        var companySearchDataJson = JsonConvert.SerializeObject(companySearchData);
        var sqlInsertQuery = $@"
            INSERT INTO public.company_search (company_search_guid, executed_by, state, company_search_data, last_message)
            VALUES ('{companySearchGuid}', '{user}', 'new', '{companySearchDataJson}', 'Starting..')";
        
        var sqlResult = ExecuteAgent("sql-execute", new List<string> { sqlInsertQuery });
        
        return ExecuteAgent("companies-search-get", new List<string> { companySearchGuid });
    }
}

class CompanySearchData
{
    public string QueryString { get; set; } = "";
    public List<string> SearchQueries { get; set; } = new List<string>();
}