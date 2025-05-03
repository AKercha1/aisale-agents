using System; 
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

class Agent 
{
    public string Run(
        Dictionary<string, string> Parameters,
        AiCoreApi.Common.RequestAccessor RequestAccessor,
        AiCoreApi.Common.ResponseAccessor ResponseAccessor,
        Func<string, List<string>?, string> ExecuteAgent,
        Func<string, string> GetCacheValue,
        Func<string, string, int, string> SetCacheValue,
        Microsoft.Extensions.Logging.ILogger<AiCoreApi.SemanticKernel.Agents.CsharpCodeAgent> Logger)
    {
        var area = Parameters["parameter1"];
        //var queriesJson = ExecuteAgent("company-search-generate-prompt", new List<string> { "2", "USA", area, "50-500", "Google" });
        var queriesJson = "{\"queries\": [\"USA swimming pool companies with 50-500 employees focused on mid-sized market growth\",\"growing swimming pool businesses in the USA with 50 to 500 employees for industry insights\"]}";
        var queries = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(queriesJson)["queries"];
        foreach (var query in queries)
        {
            //var googleSearchToolResult = ExecuteAgent("google-search-tool", new List<string> { query, "10", "0" });
            var googleSearchToolResult = "[{\"url\":\"https://www.washingtontechnology.org/health-insurance-for-early-stage-mid-market-companies/\",\"text\":\"Jan 2, 2013 ... Health Insurance and the Mid-Market. Mid-market company size is somewhat of an amorphous term, usually defined by revenue or employee size.\"},{\"url\":\"https://barfieldrevenue.com/tag/recruiting/\",\"text\":\"... the hiring goals successfully \u2014 and on time. Mid-sized Companies \u2014 50-500. Companies more in the medium size range, especially those on the smaller end of the\u00A0...\"},{\"url\":\"https://registration.events.shrm.org/flow/shrm/shrmtalent2025/attendee-portal/page/sponsors\",\"text\":\"... the talent recruitment strategy you need to staff your business in this challenging market. View. AIESEC United States/Global Current banner. Exhibitor. AIESEC\u00A0...\"},{\"url\":\"https://www.rolandberger.com/publications/publication_pdf/roland_berger_myanmar_hr_survey.pdf\",\"text\":\"Apr 5, 2025 ... 84% of small companies and 75% of mid-sized and large companies consider it as a (major) issue. ... companies employees. 50 - 500. Mid-sized.\"},{\"url\":\"https://www.investeurope.eu/media/1303/european_mm_pe-delivering_the_goods.pdf\",\"text\":\"take medium-sized business units out of much larger companies and give ... Productivity and employment growth for PE-backed companies 2005\u20132011. Source\u00A0...\"},{\"url\":\"https://www.partnersgroup.com/~/media/Files/P/Partnersgroup/Universal/shareholders/reports-and-presentations/2023/annual-report-2023.pdf\",\"text\":\"Mar 15, 2024 ... As our firm continues to grow, we remain committed to driving forward our strategy of delivering sustainable returns through a focus on\u00A0...\"},{\"url\":\"https://www.irena.org/Digital-Report/Renewable-energy-and-jobs-Annual-review-2023\",\"text\":\"4.9 million Solar photovoltaic (PV) jobs in 2022; among renewable energy technologies, solar PV is the fastest-growing sector, accounting for more than one-\u00A0...\"},{\"url\":\"https://www.linkedin.com/posts/afonso-malo-franco_icp-activity-7279451311899578368-MwdF\",\"text\":\"Dec 30, 2024 ... But as you grow and prove your hypothesis, stay focused. A few things I like to consider when articulating ICPs: - Company size (e.g. 50 - 500\u00A0...\"},{\"url\":\"https://gritsearch.com/career-advice/succeeding-at-work/pros-and-cons-of-working-in-big-vs-small-companies/\",\"text\":\"Jul 15, 2022 ... An SME is a business with less than 500 employees, whereas a big company is categorised by its size and ability to dominate in their industry.\"},{\"url\":\"https://recruitcrm.io/blogs/recruitcrm-exclusives/recruitment-funnel/\",\"text\":\"Mid-sized companies (50-500 employees). As companies grow, they must transition from founder-led hiring to more structured processes while maintaining the\u00A0...\"}]";
            var searchResults = JsonConvert.DeserializeObject<List<SearchResult>>(googleSearchToolResult);
            foreach (var searchResult in searchResults)
            {
                var uri = new Uri(searchResult.Url);            
                string[] parts = uri.Host.Split('.');
                string baseDomain = parts.Length >= 2
                    ? string.Join(".", parts[^2], parts[^1])
                    : uri.Host;
                string domainPattern = @$"^https?:\/\/([a-zA-Z0-9\-]+\.)*{Regex.Escape(baseDomain)}(?=\/|:|$)";
                ResponseAccessor.AddDebugMessage("Agent", "Debug", searchResult.Url + " " + domainPattern);

                var crawlerToolResult = ExecuteAgent("crawler-tool", new List<string> { searchResult.Url, domainPattern });
                ResponseAccessor.AddDebugMessage("Agent", "Debug", crawlerToolResult);
                return "";
            }
        }
        return "";
    }
}

class SearchResult
{
    public string Url { get; set; }
    public string Text { get; set; }
}