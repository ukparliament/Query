using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using VDS.RDF.Storage;

namespace Parliament.Data.Api.FixedQuery
{
    public class GraphDBConnector : SesameHttpProtocolVersion6Connector
    {
        private static readonly string subscriptionKey = ConfigurationManager.AppSettings["SubscriptionKey"];
        private static readonly string sparqlEndpoint = ConfigurationManager.AppSettings["SparqlEndpoint"];

        public GraphDBConnector(string endpointUri)
            :base(endpointUri, string.Empty)
        {
        }

        protected override HttpWebRequest CreateRequest(String servicePath, String accept, String method, Dictionary<String, String> queryParams)
        {
            HttpWebRequest originalRequest = base.CreateRequest(servicePath, accept, method, queryParams);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sparqlEndpoint);
            request.Method = originalRequest.Method;
            request.Timeout = originalRequest.Timeout;
            request.Accept = originalRequest.Accept;
            foreach (string key in originalRequest.Headers.AllKeys)
                if (key.ToUpper() != "ACCEPT")
                    request.Headers.Add(key, originalRequest.Headers[key]);
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            return request;
        }

    }
}