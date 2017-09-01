namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph region_index()
        {
            var queryString = base.GetSparql("region_index");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteSingle(query, "http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql");
        }

        [HttpGet]
        public Graph region_by_id(string region_code)
        {
            var queryString = base.GetSparql("region_by_id");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("regionCode", region_code);

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph region_constituencies(string region_code)
        {
            var queryString = base.GetSparql("region_constituencies");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("regionCode", region_code);

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph region_constituencies_a_to_z(string region_code)
        {
            var queryString = base.GetSparql("region_constituencies_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("regionCode", region_code);

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph region_constituencies_by_initial(string region_code, string initial)
        {
            var queryString = base.GetSparql("region_constituencies_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("regionCode", region_code);
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteSingle(query);
        }
    }
}
