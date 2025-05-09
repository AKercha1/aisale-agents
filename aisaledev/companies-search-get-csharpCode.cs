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
        // Retrieve the company search GUID from the parameters
        var companySearchGuid = Parameters["parameter1"]; 
        
        // Prepare the SQL query to fetch company search details based on the GUID
        var sqlSelectQuery = $@"select * from public.company_search where company_search_guid = '{companySearchGuid}' limit 1";
        
        // Execute the SQL query and get the result
        var sqlResult = ExecuteAgent("sql-execute", new List<string> { sqlSelectQuery });
        
        // Parse the SQL result to extract the first array element
        var parsedResult = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(sqlResult);
        
        // If there are results, return the first element as JSON
        if (parsedResult != null && parsedResult.Count > 0)
        {
            var firstResult = parsedResult[0];
            // Return only the first array element
            return JsonConvert.SerializeObject(new List<Dictionary<string, object>> { firstResult });
        }
        
        // Return an empty JSON array if no results found
        return JsonConvert.SerializeObject(new List<Dictionary<string, object>>());
    }
}