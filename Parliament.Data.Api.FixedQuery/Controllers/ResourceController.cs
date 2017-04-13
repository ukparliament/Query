namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("resources")]
    public class ResourceController : BaseController
    {
        [Route(Name = "ResourceById")]
        [HttpGet]
        public Graph ById(string uri)
        {
            var queryString = @"DESCRIBE @uri";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("uri", new Uri(uri));

            return BaseController.ExecuteSingle(query);
        }
    }
}