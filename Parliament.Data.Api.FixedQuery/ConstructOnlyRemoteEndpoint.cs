namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Net;
    using VDS.RDF.Query;

    public class ConstructOnlyRemoteEndpoint : SparqlRemoteEndpoint
    {
        public ConstructOnlyRemoteEndpoint(Uri endpointUri) : base(endpointUri)
        {
        }

        public override HttpWebResponse QueryRaw(string sparqlQuery)
        {
            var mimeTypes = new string[] { "application/n-triples", "text/turtle" };

            return base.QueryRaw(sparqlQuery, mimeTypes);
        }
    }
}