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
        string companyId = Convert.ToInt32(Parameters["parameter1"]); 
        var sqlSelectQuery = $@"select * from public.company where company_id = {companyId}";
        var sqlSelectResult = ExecuteAgent("sql-execute", new List<string> { sqlSelectQuery });
        var parsedResult = JsonConvert.DeserializeObject<List<List<CompanyItem>>>(sqlSelectResult);
        if (parsedResult != null && parsedResult.Count > 0 && parsedResult[0].Count > 0)
        {
            var firstResult = parsedResult[0][0];
            return JsonConvert.SerializeObject(firstResult);
        }        
        return "Error: No Company";
    }
}

class CompanyItem 
{
    public string name { get; set; }
    public int company_id { get; set; }
    public string url { get; set; }
    public string details { get; set; }
    public string created_by { get; set; }
    public string industry { get; set; }
    public bool company_analyzed { get; set; }
    public bool contacts_found { get; set; }
    public bool ai_opportunities_generated { get; set; }
    public bool proposal_generated { get; set; }
    public bool proposal_sent { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}