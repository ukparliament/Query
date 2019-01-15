namespace Query
{
    using System;
    using System.Configuration;
    using System.Net;
    using Microsoft.Extensions.Configuration;
    using VDS.RDF.Query;

    public class GraphDBSparqlEndpoint : SparqlRemoteEndpoint
    {
        private readonly IConfiguration configuration;
        private static readonly string apiVersion = ConfigurationManager.AppSettings["ApiVersion"];
        private static readonly string subscriptionKey = ConfigurationManager.AppSettings["SubscriptionKey"];

        public GraphDBSparqlEndpoint(IConfiguration configuration) : base(new Uri(configuration["SparqlEndpoint"]))
        {
            this.configuration = configuration;

            this.ResultsAcceptHeader = "application/sparql-results+json";
        }

        protected override void ApplyCustomRequestOptions(HttpWebRequest httpRequest)
        {
            base.ApplyCustomRequestOptions(httpRequest);
            httpRequest.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            httpRequest.Headers.Add("Api-Version", apiVersion);
        }
    }
}