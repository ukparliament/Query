namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class XController 
    {
        //[Route(Name = "ResourceById")]
        [HttpGet]
        public Graph ResourceById(string uri)
        {
            var queryString = @"DESCRIBE @uri";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("uri", new Uri(uri));

            return BaseController.ExecuteSingle(query);
        }
    }
}
