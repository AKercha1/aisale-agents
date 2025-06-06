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
        string companyItemJson = Parameters["parameter1"]; 
        var companyItemsList = JsonConvert.DeserializeObject<List<CompanyItem>>(companyItemJson);
        foreach (var companyItem in companyItemsList)
        {
            var sqlUpdateQuery = $@"
                UPDATE public.company
                SET 
                    name = '{companyItem.name.Replace("'", "")}',
                    url = '{companyItem.url.Replace("'", "")}',
                    details = '{companyItem.details.Replace("'", "")}',
                    created_by = '{companyItem.created_by.Replace("'", "")}',
                    industry = '{companyItem.industry?.Replace("'", "") ?? ""}',
                    company_analyzed = {(companyItem.company_analyzed.HasValue ? (companyItem.company_analyzed.Value ? "true" : "false") : "false")},
                    contacts_found = {(companyItem.contacts_found.HasValue ? (companyItem.contacts_found.Value ? "true" : "false") : "false")},
                    ai_opportunities_generated = {(companyItem.ai_opportunities_generated.HasValue ? (companyItem.ai_opportunities_generated.Value ? "true" : "false") : "false")},
                    proposal_generated = {(companyItem.proposal_generated.HasValue ? (companyItem.proposal_generated.Value ? "true" : "false") : "false")},
                    updated_at = now()
                WHERE company_id = {companyItem.company_id};";
            ExecuteAgent("sql-execute", new List<string> { sqlUpdateQuery });
        }
        return "OK";
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
    public bool? company_analyzed { get; set; }
    public bool? contacts_found { get; set; }
    public bool? ai_opportunities_generated { get; set; }
    public bool? proposal_generated { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}