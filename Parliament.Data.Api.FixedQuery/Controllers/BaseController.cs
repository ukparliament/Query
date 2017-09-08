namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Parsing.Handlers;
    using VDS.RDF.Query;
    using VDS.RDF.Storage;

    // TODO: Merge with FixedQueryController
    public abstract partial class BaseController : ApiController
    {
        private static readonly string sparqlEndpoint = ConfigurationManager.AppSettings["SparqlEndpoint"];
        private static readonly string subscriptionKey = ConfigurationManager.AppSettings["SubscriptionKey"];
        private static readonly string endpointUri = $"{sparqlEndpoint}?subscription-key={subscriptionKey}";
        // TODO: Extract to config or elsewhere
        protected static readonly Uri instance = new Uri("http://id.ukpds.org/");
        protected static readonly Uri schema = new Uri(instance, "schema/");

        protected static Graph ExecuteSingle(SparqlParameterizedString query)
        {
            return ExecuteSingle(query, endpointUri);
        }

        protected static Graph ExecuteSingle(SparqlParameterizedString query, string endpointUri)
        {
            var result = ExecuteList(query, endpointUri);

            if (result.IsEmpty)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return result;
        }

        protected static Graph ExecuteList(SparqlParameterizedString query)
        {
            return ExecuteList(query, endpointUri);
        }

        protected static Graph ExecuteList(SparqlParameterizedString query, string endpointUri)
        {
            // TODO: This should move to controller action
            query.SetUri("schemaUri", schema);

            // TODO: This should move to controller action
            var queryString = query.ToString();

            // TODO: This should be registered for disposal
            var graph = new Graph();

            graph.NamespaceMap.AddNamespace("owl", new Uri("http://www.w3.org/2002/07/owl#"));
            graph.NamespaceMap.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            graph.NamespaceMap.AddNamespace("id", FixedQueryController.instance);
            graph.NamespaceMap.AddNamespace("schema", FixedQueryController.schema);

            var graphHandler = new GraphHandler(graph);

            var endpoint = new ConstructOnlyRemoteEndpoint(new Uri(endpointUri));
            using (var connector = new SparqlConnector(endpoint))
            {
                connector.Query(graphHandler, null, queryString);
            }

            return graph;
        }
    }
}