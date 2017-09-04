namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph contact_point_index()
        {
            var queryString = base.GetSparql("contact_point_index");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph contact_point_by_id(string contact_point_id)
        {
            var queryString = base.GetSparql("contact_point_by_id");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(instance, contact_point_id));

            return BaseController.ExecuteSingle(query);
        }
    }
}
