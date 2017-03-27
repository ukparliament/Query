namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Configuration;
    using System.Net.Http;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Parsing.Handlers;
    using VDS.RDF.Query;

    public abstract class BaseController : ApiController
    {
        protected static readonly Uri instance = new Uri("http://id.ukpds.org/");
        protected static readonly Uri schema = new Uri(instance, "schema/");

        protected HttpResponseMessage Execute(SparqlParameterizedString query)
        {
            return Execute(query.ToString());
        }

        protected HttpResponseMessage Execute(string query)
        {
            Graph graph = runQuery(query);

            return this.Request.CreateResponse(graph.IsEmpty ? System.Net.HttpStatusCode.NoContent : System.Net.HttpStatusCode.OK, graph);
        }

        protected HttpResponseMessage Execute(string query1, string query2)
        {
            Graph graph1 = runQuery(query1);
            Graph graph2 = runQuery(query2);
            graph1.Merge(graph2);


            return this.Request.CreateResponse(graph1.IsEmpty ? System.Net.HttpStatusCode.NoContent : System.Net.HttpStatusCode.OK, graph1);
        }

        private Graph runQuery(string query)
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