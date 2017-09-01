namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph resource_by_id(string uri)
        {
            var queryString = @"DESCRIBE @uri";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("uri", new Uri(uri));

            return BaseController.ExecuteSingle(query);
        }
    }
}
