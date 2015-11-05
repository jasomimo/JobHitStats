using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Threading;
using System.Diagnostics;

namespace JobHitStats
{
    using JobHitStats.Common;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        private static readonly IEnumerable<JobPortal> JobPortals = Enum.GetValues(typeof(JobPortal)).Cast<JobPortal>();
        private static readonly IEnumerable<Technology> Technologies = Enum.GetValues(typeof(Technology)).Cast<Technology>();

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            Logger.LogConsole = false;
            Logger.LogStart("Job Hit Stats");

            foreach (JobPortal portal in JobPortals)
            {
                Logger.LogInfo(string.Format("Going to process '{0}' portal.", portal));

                IDictionary<Technology, uint> jobOffers = GetJobOffers(portal);

                DataStorage ds = null;
                try
                {
                    ds = new DataStorage();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Unable to connect to SQLite database.", Logger.MethodName);

                    // TODO: send email, backup data into some temp file
                }

                bool dataStored = false;
                try
                {
                    ds.StoreData(portal, jobOffers);
                    dataStored = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Unable to store data into SQLite database.", Logger.MethodName);

                    // TODO: send email, backup data into some temp file
                }

                if (dataStored) 
                { 
                    ds.WriteTableContentToConsole(portal);
                }
            }

            Logger.LogStop("Job Hit Stats");
        }

        private static IDictionary<Technology, uint> GetJobOffers(JobPortal portal)
        {
            Logger.LogStart(Logger.MethodName);

            Logger.LogToConsole(string.Format("Crawling data for {0}", portal));

            IDictionary<Technology, uint> jobOffers = new Dictionary<Technology, uint>();
            
            foreach (Technology technology in Technologies)
            {
                string term = HandleSpecialCases(portal, SearchTermProvider.GetSearchTerms(technology));

                string content = null;
                try
                {
                    content = Crawler.Crawl(portal, term);
                }
                catch (Exception ex)
                {
                    string errorMsg = string.Format("Unable to crawl '{0}' from {1}.", term, portal);
                    errorMsg += "\nDetail: " + ex.Message;
                    Logger.LogError(ex, errorMsg, Logger.MethodName); // TODO: send email

                    continue;
                }

                uint jobsCount = 0;
                try
                {
                    jobsCount = Parser.ParseJobOffersCount(portal, content);
                }
                catch (Exception ex)
                {
                    string errorMsg = string.Format("Unable to parse job count for '{0}' from {1}.", term, portal);
                    errorMsg += "\nDetail: " + ex.Message;
                    Logger.LogError(ex, errorMsg, Logger.MethodName); // TODO: send email

                    continue;
                }

                jobOffers.Add(technology, jobsCount);
                
                Logger.LogToConsole(string.Format("For {0}, there is currently {1} jobs.", term, jobsCount));

                int randomDelay = RequestDelay();
                Thread.Sleep(randomDelay);
            }

            Logger.LogStop(Logger.MethodName);

            return jobOffers;
        }

        private static string HandleSpecialCases(JobPortal portal, string searchTerm)
        {
            switch (portal)
            {
                case JobPortal.Profesia:
                    return searchTerm.Equals("Objective-C") ? "Objective C" : searchTerm;
                
                default:
                    return searchTerm;
            }
        }

        private static int RequestDelay(int min = 2000, int max = 5000)
        {
            Random generator = new Random();
            return generator.Next(min, max);
        }

        private static IDictionary<Technology, uint> PrepareData()
        {
            IDictionary<Technology, uint> data = new Dictionary<Technology, uint>();

            data.Add(Technology.Abap, 1);
            data.Add(Technology.AngularJS, 2);
            data.Add(Technology.Assembler, 3);
            data.Add(Technology.BackboneJS, 41);
            data.Add(Technology.Bootstrap, 1);
            data.Add(Technology.Cobol, 1);
            data.Add(Technology.CPlusPlus, 1);
            data.Add(Technology.CSharp, 1);
            data.Add(Technology.Css, 1);
            data.Add(Technology.Fortran, 1);
            data.Add(Technology.Html, 1);
            data.Add(Technology.Java, 1);
            data.Add(Technology.JavaScript, 999);
            data.Add(Technology.Json, 1);
            data.Add(Technology.Matlab, 1);
            data.Add(Technology.Pascal, 1);
            data.Add(Technology.Perl, 1);
            data.Add(Technology.Php, 1);
            data.Add(Technology.PowerShell, 1);
            data.Add(Technology.Python, 1);
            data.Add(Technology.Ruby, 1);
            data.Add(Technology.Scala, 32);
            data.Add(Technology.SharePoint, 0);
            data.Add(Technology.Sitecore, 1);
            data.Add(Technology.Sql, 32);
            //data.Add(Technology.VisualBasic, 3);
            data.Add(Technology.Xml, 43);

            return data;
        }
    }
}
