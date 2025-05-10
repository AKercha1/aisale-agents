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

        var sqlUpdateQuery = $@"
            update public.company_search set company_search_data = '{companySearchDataJson}' where company_search_guid = '{companySearchGuid}'";
        
        var sqlResult = ExecuteAgent("sql-execute", new List<string> { sqlUpdateQuery });

        return ExecuteAgent("companies-search-get", new List<string> { companySearchGuid });
    }
}