using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobHitStats.Common
{
    using System.Diagnostics;

    internal static class Logger
    {
        private const int DefaultId = 0;

        private static readonly TraceSource Source = new TraceSource("JobHitStats", SourceLevels.Information);

        private static bool logConsole = false;

        public static bool LogConsole 
        {
            get
            {
                return logConsole;
            }

            set
            {
                logConsole = value;
            }
        }

        public static string MethodName
        {
            get
            {
                return new StackFrame(1).GetMethod().Name;
            }
        }

        public static void LogInfo(string message)
        {
            try
            {
                Source.TraceEvent(TraceEventType.Information, DefaultId, message);
                Source.Flush();
            }
            catch (Exception)
            {
                // TODO: Log to event log
                throw;
            }
        }

        public static void LogWarning(string message)
        {
            try
            {
                Source.TraceEvent(TraceEventType.Warning, DefaultId, message);
                Source.Flush();
            }
            catch (Exception)
            {
                // TODO: Log to event log
                throw;
            }
        }

        public static void LogError(Exception exception, string customMessage = "", string origin = "")
        {
            try
            {
                string exceptionMessage = exception != null ? exception.Message : string.Empty;
                string errorMessage = string.Format("ERROR:\nOrigin: '{0}'\nCustom customMessage: '{1}'\nException customMessage: '{2}'", origin, customMessage, exceptionMessage);
                
                Source.TraceEvent(TraceEventType.Error, DefaultId, errorMessage);
                Source.Flush();
            }
            catch (Exception)
            {
                // TODO: Log to event log
                throw;
            }
        }

        public static void LogStart(string startOperation)
        {
            Source.TraceEvent(TraceEventType.Start, DefaultId, startOperation);
            Source.Flush();
        }

        public static void LogStop(string endOperation)
        {
            Source.TraceEvent(TraceEventType.Stop, DefaultId, endOperation);
            Source.Flush();
        }

        public static void LogToConsole(string messageToLog)
        {
            if (logConsole)
            {
                Console.WriteLine(messageToLog);
            }
        }
    }
}
