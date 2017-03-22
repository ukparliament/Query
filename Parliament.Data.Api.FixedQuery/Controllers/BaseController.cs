namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net.Http;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;
    using VDS.RDF.Storage;

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
            Graph graph;
            var endpoint = new Uri(ConfigurationManager.AppSettings["SparqlEndpoint"]);
            using (var connector = new SparqlConnector(endpoint))
            {
                graph = connector.Query(query.ToString()) as Graph;
            }

            graph.NamespaceMap.AddNamespace("id", ConstituencyController.instance);
            graph.NamespaceMap.AddNamespace("schema", ConstituencyController.schema);
            return graph;
        }
    }
}