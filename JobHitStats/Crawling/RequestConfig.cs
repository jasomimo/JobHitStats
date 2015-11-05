using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobHitStats
{
    internal class RequestConfig
    {
        public Uri BaseUrl { get; set; }
        
        public string Resource { get; set; }
        
        public IDictionary<string, object> QueryStringParameters { get; private set; }

        public RequestConfig()
        {
            QueryStringParameters = new Dictionary<string, object>();
        }

        public void SetSearchTerm(string key, string value)
        {
            QueryStringParameters.Add(key, value);
        }
    }
}
