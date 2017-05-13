namespace Parliament.Data.Api.FixedQuery
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net;
    using VDS.RDF.Storage;

    public class GraphDBConnector : SesameHttpProtocolVersion6Connector
    {
        private static readonly string subscriptionKey = ConfigurationManager.AppSettings["SubscriptionKey"];
        private static readonly string sparqlEndpoint = ConfigurationManager.AppSettings["SparqlEndpoint"];

        public GraphDBConnector(string endpointUri) : base(endpointUri, string.Empty) { }

        protected override HttpWebRequest CreateRequest(string servicePath, string accept, string method, Dictionary<string, string> queryParams)
        {
            var originalRequest = base.CreateRequest(servicePath, accept, method, queryParams);

            var request = (HttpWebRequest)WebRequest.Create(sparqlEndpoint);
            request.Method = originalRequest.Method;
            request.Timeout = originalRequest.Timeout;
            request.Accept = originalRequest.Accept;

            foreach (string key in originalRequest.Headers.AllKeys)
            {
                if (key.ToUpperInvariant() != "ACCEPT")
                {
                    request.Headers.Add(key, originalRequest.Headers[key]);
                }
            }

            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            return request;
        }

    }
}