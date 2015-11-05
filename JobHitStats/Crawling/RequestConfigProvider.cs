using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Contrib;
using System.Text.RegularExpressions;

namespace JobHitStats
{
    internal static class RequestConfigProvider
    {
        public static RequestConfig GetRequestConfig(JobPortal portal, string searchTerm)
        {
            switch(portal)
            {
                case JobPortal.Profesia:
                    return GetProfesiaRequestConfig(searchTerm);

                case JobPortal.CareersStackOverflow:
                    return GetCareersStackOverflowRequestConfig(searchTerm);

                case JobPortal.Karriere:
                    return GetKarriereRequestConfig(searchTerm);

                case JobPortal.CWJobs:
                    return GetCWJobsRequestConfig(searchTerm);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Provides default configuration for Profesia request, without search term initialized.
        /// </summary>
        /// <returns></returns>
        private static RequestConfig GetProfesiaRequestConfig(string searchTerm)
        {
            RequestConfig requestConfig = new RequestConfig();

            requestConfig.BaseUrl = new Uri(@"http://www.profesia.sk");

            requestConfig.Resource = @"search.php?" + 
                "log_term={log_term}&" + 
                "which_form={which_form}&" + 
                "cnt_offered={cnt_offered}&" +
                "search_anywhere={search_anywhere}&" + 
                "search_submit={search_submit}";

            searchTerm = SearchTermHandler(searchTerm);

            // fixed parameters
            requestConfig.QueryStringParameters.Add("log_term", "1");
            requestConfig.QueryStringParameters.Add("which_form", "search_page");
            requestConfig.QueryStringParameters.Add("cnt_offered", "1");
            requestConfig.QueryStringParameters.Add("search_anywhere", searchTerm);
            requestConfig.QueryStringParameters.Add("search_submit", "H%C4%BEada%C5%A5");

            return requestConfig;
        }

        private static RequestConfig GetCareersStackOverflowRequestConfig(string searchTerm)
        {
            RequestConfig requestConfig = new RequestConfig();

            requestConfig.BaseUrl = new Uri(@"http://careers.stackoverflow.com");
            requestConfig.Resource = @"jobs?searchTerm={searchTerm}";

            searchTerm = SearchTermHandler(searchTerm);
            requestConfig.QueryStringParameters.Add("searchTerm", searchTerm);

            return requestConfig;
        }

        private static RequestConfig GetKarriereRequestConfig(string searchTerm)
        {
            RequestConfig requestConfig = new RequestConfig();

            requestConfig.BaseUrl = new Uri(@"http://www.karriere.at");
            requestConfig.Resource = @"jobs/{searchTerm}";

            searchTerm = SearchTermHandler(searchTerm);
            requestConfig.QueryStringParameters.Add("searchTerm", searchTerm);

            return requestConfig;
        }

        private static RequestConfig GetCWJobsRequestConfig(string searchTerm)
        {
            RequestConfig requestConfig = new RequestConfig();

            requestConfig.BaseUrl = new Uri(@"http://www.cwjobs.co.uk");
            requestConfig.Resource = @"JobSearch/Results.aspx?Keywords={searchTerm}";

            searchTerm = SearchTermHandler(searchTerm);
            requestConfig.QueryStringParameters.Add("searchTerm", searchTerm);

            return requestConfig;
        }

        private static string SearchTermHandler(string searchTerm)
        {
            if (Regex.IsMatch(searchTerm, "\\s"))
            {
                searchTerm = "\"" + searchTerm + "\"";
            }
            
            //searchTerm = HttpUtility.UrlEncode(searchTerm);

            return searchTerm;
        }
    }
}
