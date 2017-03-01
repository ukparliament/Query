namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("houses")]
    public class HouseController : BaseController
    {
        // Ruby route: match '/houses/:house', to: 'houses#show', house: /\w{8}-\w{4}-\w{4}-\w{4}-\w{12}/, via: [:get]

        [Route("{id:guid}", Name = "HouseById")]
        [HttpGet]
        public Graph ById(string id)
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?house
        a :House ;
        :houseName ?houseName .
}
WHERE {
    BIND(@id AS ?house)
    ?house 
        a :House ;
        :houseName ?houseName .
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, id));

            return BaseController.Execute(query);

        }
    }
}
