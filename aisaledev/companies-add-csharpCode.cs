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
        var source = Parameters["parameter1"].Replace("'", "");
        var sourceId = Parameters["parameter2"].Replace("'", "");
        var companiesListJson = Parameters["parameter3"]; 
        var user = RequestAccessor.Login.Replace("'", "");
        var companiesList = JsonConvert.DeserializeObject<List<CompaniesListItem>>(companiesListJson);

        var companyNamesJson = ExecuteAgent("companies-get-names", new List<string> { });
        var companyNamesList = JsonConvert.DeserializeObject<List<string>>(companyNamesJson);
        
        foreach(var companyItem in companiesList)
        {
            if(companyNamesList.Contains(companyItem.name))
                continue;
            var sqlInsertQuery = $@"
                INSERT INTO public.company (source, source_id, created_by, name, url, details)
                VALUES ('{source}', '{sourceId}', '{user}', '{companyItem.name.Replace("'", "")}', '{companyItem.url.Replace("'", "")}', '{companyItem.details.Replace("'", "")}')";
            var sqlInsertResult = ExecuteAgent("sql-execute", new List<string> { sqlInsertQuery });
        }        
        return "OK";
    }
}

class NameItem 
{
    public string name { get; set; }
}

class CompaniesListItem 
{
    public string name { get; set; }
    public string url { get; set; }
    public string details { get; set; }
}