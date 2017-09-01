namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph image_by_id(string image_id)
        {
            var queryString = base.GetSparql("image_by_id");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, image_id));

            return BaseController.ExecuteSingle(query);
        }
    }
}
