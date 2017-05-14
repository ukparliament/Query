namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Parsing.Handlers;
    using VDS.RDF.Parsing.Validation;
    using VDS.RDF.Query;
    using VDS.RDF.Storage;

    public abstract class BaseController : ApiController
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
            var queryString = query.ToString();

            ValidateSparql(queryString);

            var graph = new Graph();

            graph.NamespaceMap.AddNamespace("id", ConstituencyController.instance);
            graph.NamespaceMap.AddNamespace("schema", ConstituencyController.schema);

            var graphHandler = new GraphHandler(graph);

            using (var connector = new SparqlConnector(new Uri(endpointUri)) )
            {
                connector.SkipLocalParsing = true; // This was already done above

                connector.Query(graphHandler, null, queryString);
            }

            return graph;
        }

        private static void ValidateSparql(string query)
        {
            var validator = new SparqlQueryValidator();
            var result = validator.Validate(query);

            if (!result.IsValid)
            {
                throw new SparqlInvalidException(result.Message);
            }
        }
    }
}