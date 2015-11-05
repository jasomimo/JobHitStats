using RestSharp;

namespace JobHitStats
{
    using JobHitStats.Common;

    internal static class Crawler
    {
        public static string Crawl(JobPortal portal, string searchTerm)
        {
            Logger.LogStart(Logger.MethodName);

            RequestConfig requestConfig = RequestConfigProvider.GetRequestConfig(portal, searchTerm);

            RestClient client = new RestClient();
            client.BaseUrl = requestConfig.BaseUrl;

            RestRequest request = new RestRequest();
            request.Resource = requestConfig.Resource;

            foreach (string key in requestConfig.QueryStringParameters.Keys)
            {
                request.AddParameter(key, requestConfig.QueryStringParameters[key], ParameterType.UrlSegment);
            }

            string urlToCrawl = client.BaseUrl.ToString() + request.Resource;
            Logger.LogStart("Crawling " + urlToCrawl);
            
            IRestResponse resposne = client.Execute(request);

            Logger.LogStop("Crawling finished.");

            if (resposne.ErrorException != null)
            {
                throw resposne.ErrorException;
            }

            Logger.LogStop(Logger.MethodName);

            return resposne.Content;
        }
    }
}
