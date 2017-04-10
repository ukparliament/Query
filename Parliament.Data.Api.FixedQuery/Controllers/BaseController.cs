namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Configuration;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;
    using VDS.RDF.Storage;
    using VDS.RDF.Parsing.Handlers;

    public abstract class BaseController : ApiController
    {
        protected static readonly Uri instance = new Uri("http://id.ukpds.org/");
        protected static readonly Uri schema = new Uri(instance, "schema/");

        // TODO: Implement single vs list throughout
        protected static Graph ExecuteSingle(SparqlParameterizedString query)
        {
            var result = BaseController.Execute(query);
            if (result.IsEmpty)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            return result;
        }

        // TODO: rename to list
        protected static Graph Execute(SparqlParameterizedString query)
        {
            return BaseController.ExecuteList(query.ToString());
        }

        // TODO: rename to list
        protected static Graph ExecuteList(string query)
        {
            IGraph graph = new Graph();
            var endpointUri = ConfigurationManager.AppSettings["SparqlEndpoint"];

            graph.NamespaceMap.AddNamespace("id", ConstituencyController.instance);
            graph.NamespaceMap.AddNamespace("schema", ConstituencyController.schema);
            using (GraphDBConnector connector = new GraphDBConnector(endpointUri))
            {
                GraphHandler rdfHandler = new GraphHandler(graph);
                connector.Query(rdfHandler, null, query);
            }

            return graph as Graph;
        }
    }
}