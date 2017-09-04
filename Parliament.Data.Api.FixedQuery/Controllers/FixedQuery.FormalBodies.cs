namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph formal_body_index()
        {
            var queryString = base.GetSparql("formal_body_index");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph formal_body_by_id(string formal_body_id)
        {
            var queryString = base.GetSparql("formal_body_by_id");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, formal_body_id));

            return BaseController.ExecuteSingle(query);

        }

        [HttpGet]
        public Graph formal_body_membership(string formal_body_id)
        {
            var queryString = base.GetSparql("formal_body_membership");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, formal_body_id));

            return BaseController.ExecuteList(query);
        }

    }
}
