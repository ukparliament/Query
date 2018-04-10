using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using VDS.RDF.Query;
using VDS.RDF.Storage;

namespace Parliament.Data.Api.FixedQuery
{
    public class GraphDBSparqlEndpoint : SparqlRemoteEndpoint
    {
        private static readonly string sparqlEndpoint = ConfigurationManager.AppSettings["SparqlEndpoint"];
        private static readonly string apiVersion = ConfigurationManager.AppSettings["ApiVersion"];
        private static readonly string subscriptionKey = ConfigurationManager.AppSettings["SubscriptionKey"];
        private static readonly Uri endpoint = new Uri(sparqlEndpoint);

        public GraphDBSparqlEndpoint():base(endpoint)
        {
        }

        protected override void ApplyCustomRequestOptions(HttpWebRequest httpRequest)
        {
            base.ApplyCustomRequestOptions(httpRequest);
            httpRequest.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            httpRequest.Headers.Add("Api-Version", apiVersion);
        }
    }
}