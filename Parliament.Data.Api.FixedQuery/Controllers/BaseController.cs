namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;
    using VDS.RDF.Storage;

    // TODO: Merge with FixedQueryController
    public abstract partial class BaseController : ApiController
    {
        private static readonly string sparqlEndpoint = ConfigurationManager.AppSettings["SparqlEndpoint"];
        private static readonly string subscriptionKey = ConfigurationManager.AppSettings["SubscriptionKey"];
        private static readonly string endpointUri = $"{sparqlEndpoint}?subscription-key={subscriptionKey}";

        protected static object ExecuteSingle(SparqlParameterizedString query)
        {
            return ExecuteSingle(query, endpointUri);
        }

        protected static object ExecuteSingle(SparqlParameterizedString query, string endpointUri)
        {
            var result = ExecuteList(query, endpointUri);

            if (result is IGraph graph && graph.IsEmpty || result is SparqlResultSet resultSet && resultSet.IsEmpty)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return result;
        }

        protected static object ExecuteList(SparqlParameterizedString query)
        {
            return ExecuteList(query, endpointUri);
        }

        protected static object ExecuteList(SparqlParameterizedString query, string endpointUri)
        {
            // TODO: This should move to controller action
            var queryString = query.ToString();

            using (var connector = new SparqlConnector(new Uri(endpointUri)))
            {
                var results = connector.Query(queryString);

                if (results is IGraph)
                {
                    AddNamespaces(results as IGraph);
                }

                return results;
            }
        }

        private static void AddNamespaces(IGraph graph)
        {
            graph.NamespaceMap.AddNamespace("owl", new Uri("http://www.w3.org/2002/07/owl#"));
            graph.NamespaceMap.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            graph.NamespaceMap.AddNamespace("id", Global.InstanceUri);
            graph.NamespaceMap.AddNamespace("schema", Global.SchemaUri);
        }
    }
}