using System;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace JobHitStats
{
    using System.Globalization;

    using JobHitStats.Common;

    internal static class Parser
    {
        private const string ProfesiaJobOffersXPath = @"//dd[@class='first active space-on-phones']";

        private const string CareersStackOverflowJobOffersXPath = @"//div[@id='index-hed']//h2";

        private const string KarriereJobOffersXPath = @"//header[@id='jobsSearchresultsHeader']/h1";

        private const string CwJobsJobOffersXPath = @"//span[@class='page-header-job-count']";

        public static uint ParseJobOffersCount(JobPortal portal, string htmlContent)
        {
            Logger.LogStart(Logger.MethodName);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            string xPath = GetXPath(portal);
            HtmlNodeCollection jobOffersElement = doc.DocumentNode.SelectNodes(xPath);

            if (jobOffersElement == null || jobOffersElement.Count != 1)
            {
                string jobOfferElementsCount = jobOffersElement == null ? "null" : jobOffersElement.Count.ToString(CultureInfo.InvariantCulture);
                throw new Exception(string.Format("For portal '{0}' found none or more than 1 job offer elements. Job offer elements: " + jobOfferElementsCount, portal));
            }

            HtmlNode jobOffers = jobOffersElement[0];

            Logger.LogInfo("Job offers inner text: " + jobOffers.InnerText);

            string texCount = GetJobOffersTextToParse(portal, jobOffers.InnerText);
            if (texCount == null)
            {
                Logger.LogWarning("Cannot get job count from inner text.");
                return 0;   // assume there is 0 jobs
            }

            Logger.LogInfo("Job offers text to parse: " + texCount);

            uint count;
            if (uint.TryParse(texCount, out count))
            {
                Logger.LogStop(Logger.MethodName);

                return count;
            }

            Logger.LogStop(Logger.MethodName);

            throw new Exception(string.Format("Cannot parse integer from '{0}'.", jobOffers.InnerText));
        }

        private static string GetXPath(JobPortal portal)
        {
            switch (portal)
            {
                case JobPortal.Profesia:
                    return ProfesiaJobOffersXPath;

                case JobPortal.CareersStackOverflow:
                    return CareersStackOverflowJobOffersXPath;

                case JobPortal.Karriere:
                    return KarriereJobOffersXPath;

                case JobPortal.CWJobs:
                    return CwJobsJobOffersXPath;

                default:
                    return null;
            }
        }

        private static string GetJobOffersTextToParse(JobPortal portal, string innerText)
        {
            switch (portal)
            {
                case JobPortal.Profesia:
                    int leftBracket = innerText.IndexOf("(");
                    int rightBracket = innerText.IndexOf(")");
                    
                    return innerText.Substring(leftBracket + 1, rightBracket - (leftBracket + 1));

                case JobPortal.CareersStackOverflow:
                case JobPortal.Karriere:
                case JobPortal.CWJobs:
                    innerText = innerText.Trim();
                    return GetTextCountForCwJobs(innerText);

                default: 
                    return null;
            }
        }

        private static string GetTextCountForCwJobs(string innerText)
        {
            string temp = Regex.Replace(innerText, "\\s*[,.]\\s*", string.Empty); // removes ',' in 1,234
            Match match = Regex.Match(temp, @"\d+");
            
            return match.Success ? match.Value : null;
        }
    }
}
