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
        var sqlSelectQuery = $@"select * from public.company_search where company_search_guid = '{companySearchGuid}' limit 1";
        var sqlResult = ExecuteAgent("sql-execute", new List<string> { sqlSelectQuery });
        
        var parsedResult = JsonConvert.DeserializeObject<List<object>>(sqlResult);
        
        if (parsedResult != null && parsedResult.Count > 0)
        {
            var firstResult = parsedResult[0];
            return JsonConvert.SerializeObject(firstResult);
        }        
        return "Incorrect companySearchGuid"
    }
}