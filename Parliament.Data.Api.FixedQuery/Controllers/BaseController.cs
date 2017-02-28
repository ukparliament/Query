namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Configuration;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;
    using VDS.RDF.Storage;

    public abstract class BaseController : ApiController
    {
        protected static readonly Uri instance = new Uri("http://id.ukpds.org/");
        protected static readonly Uri schema = new Uri(instance, "schema/");

        protected static Graph Execute(SparqlParameterizedString query)
        {
            return BaseController.Execute(query.ToString());
        }

        protected static Graph Execute(string query)
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