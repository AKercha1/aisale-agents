using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

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
        var sqlSelectQuery = @"select name from public.company";
        var sqlSelectResult = ExecuteAgent("sql-execute", new List<string> { sqlSelectQuery });
        var parsedResult = JsonConvert.DeserializeObject<List<List<NameItem>>>(sqlSelectResult);
        if (parsedResult != null && parsedResult.Count > 0)
        {
            var firstResult = parsedResult[0].Select(x => x.name).ToList();
            return JsonConvert.SerializeObject(firstResult);
        }        
        return "[]";
    }
}

class NameItem 
{
    public string name { get; set; }
}