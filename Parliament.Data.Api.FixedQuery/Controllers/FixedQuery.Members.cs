namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph member_current()
        {
            var queryString = base.GetSparql("member_current");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph member_by_initial(string initial)
        {
            var queryString = base.GetSparql("member_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);

        }

        [HttpGet]
        public Graph member_a_to_z()
        {
            var queryString = base.GetSparql("member_a_to_z");

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph member_current_by_initial(string initial)
        {
            var queryString = base.GetSparql("member_current_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph member_current_a_to_z()
        {
            var queryString = base.GetSparql("member_current_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }
    }
}
