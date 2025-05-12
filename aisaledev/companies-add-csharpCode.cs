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
        var companiesListJson = Parameters["parameter2"]; 
        var companiesList = JsonConvert.DeserializeObject<List<CompaniesListItem>>(companiesListJson);

        return "Hello World!";
    }
}

class CompaniesListItem 
{
    public string name { get; set; }
    public string url { get; set; }
    public string details { get; set; }
}