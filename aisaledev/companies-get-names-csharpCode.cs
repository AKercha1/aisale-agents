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
        var companySearchGuid = Parameters["parameter1"].Replace("'", "");
        var companiesListJson = Parameters["parameter2"]; 
        var user = RequestAccessor.Login.Replace("'", "");
        var companiesList = JsonConvert.DeserializeObject<List<CompaniesListItem>>(companiesListJson);

        var sqlSelectQuery = @"select name from public.company";
        var sqlSelectResult = ExecuteAgent("sql-execute", new List<string> { sqlSelectQuery });
        var namesList = JsonConvert.DeserializeObject<List<NameItem>>(sqlSelectResult).Select(x => x.name.ToUpper()).ToList();
        
        foreach(var companyItem in companiesList)
        {
            if(namesList.Contains(companyItem.name.ToUpper()))
                continue;
            var sqlInsertQuery = $@"
                INSERT INTO public.company (source, source_id, created_by, name, url, details)
                VALUES ('search', '{companySearchGuid}', '{user}', '{companyItem.name.Replace("'", "")}', '{companyItem.url.Replace("'", "")}', '{companyItem.details.Replace("'", "")}')";
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