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
        var companiesSearchJson = Parameters["parameter1"]; 
        var companiesSearch = JsonConvert.DeserializeObject<CompanySearchItem>(companiesSearchJson);

        var sqlSelectQuery = $@"select * from public.company_search where company_search_guid = '{companySearchGuid}' limit 1";
        var sqlResult = ExecuteAgent("sql-execute", new List<string> { sqlSelectQuery });
        
        var parsedResult = JsonConvert.DeserializeObject<List<List<object>>>(sqlResult);
        
        if (parsedResult != null && parsedResult.Count > 0 && parsedResult[0].Count > 0)
        {
            var firstResult = parsedResult[0][0];
            return JsonConvert.SerializeObject(firstResult);
        }        
        return "Incorrect companySearchGuid";
    }
}


class CompanySearchItem
{
    public int company_search_id { get; set; }
    public string company_search_guid  { get; set; }
    public DateTime executed_at { get; set; }
    public string executed_by { get; set; }
    public string state { get; set; }
    public CompanySearchData company_search_data  { get; set; }
    public string last_message { get; set; }
}

class CompanySearchData
{
    public string QueryString { get; set; } = "";
    public int? SearchDepth { get; set; }
    public List<string> SearchQueries { get; set; } = new List<string>();
}