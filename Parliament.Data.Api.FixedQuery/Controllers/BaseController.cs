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

    public abstract class BaseController : ApiController
    {
        protected static readonly Uri instance = new Uri("http://id.ukpds.org/");
        protected static readonly Uri schema = new Uri(instance, "schema/");

        protected static Graph ExecuteSingle(SparqlParameterizedString query)
        {
            var endpointUri = ConfigurationManager.AppSettings["SparqlEndpoint"];

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

        protected static Graph ExecuteList(SparqlParameterizedString query) {
            var endpointUri = ConfigurationManager.AppSettings["SparqlEndpoint"];

            return ExecuteList(query, endpointUri);
        }

        protected static Graph ExecuteList(SparqlParameterizedString query, string endpointUri)
        {
            var queryString = query.ToString();

            ValidateSparql(queryString);

            var graph = new Graph();

            graph.NamespaceMap.AddNamespace("id", ConstituencyController.instance);
            graph.NamespaceMap.AddNamespace("schema", ConstituencyController.schema);

            using (var connector = new GraphDBConnector(endpointUri))
            {
                GraphHandler rdfHandler = new GraphHandler(graph);
                connector.Query(rdfHandler, null, queryString);
            }

            return graph as Graph;
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