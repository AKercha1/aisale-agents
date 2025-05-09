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
        string companySearchGuid = Parameters["parameter1"]; 
        string user = RequestAccessor.Login;
        
        string sqlInsertQuery = $@"
            INSERT INTO public.company_search (company_search_guid, executed_by, state, company_search_data)
            VALUES ('{companySearchGuid}', '{user}', 'new', '{{}}')";
        
        var sqlResult = ExecuteAgent("sql-execute", new List<string> { sqlInsertQuery });
        
        return sqlResult;
    }
}