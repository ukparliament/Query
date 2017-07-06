namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class XController 
    {
        [HttpGet]
        public Graph ConstituencyById()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?s ?p ?o
}
WHERE {
    ?s ?p ?o.
}
LIMIT 1
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteSingle(query);
        }
    }
}
