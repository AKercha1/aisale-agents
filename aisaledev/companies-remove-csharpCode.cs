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
        string companyId = Parameters["parameter1"].Replace("'", ""); 
        var sqlDeleteQuery = @$"delete from public.company where company_id = {companyId}";
        ExecuteAgent("sql-execute", new List<string> { sqlDeleteQuery });
        return "OK";
    }
}