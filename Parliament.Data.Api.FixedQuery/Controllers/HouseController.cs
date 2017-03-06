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

        // Ruby route: get '/houses/lookup', to: 'houses#lookup'
        // NOTE: this does not work as we don't have house mnis IDs, as that feels like overkill, and overreliance on MNIS
        // could potentially just become a lookup by name?

        [Route("lookup/{source:alpha}/{id}", Name = "HouseLookup")]
        [HttpGet]
        public Graph Lookup(string source, string id)
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?house a :House .
}
WHERE {
    BIND(@id AS ?id)
    BIND(@source AS ?source)

    ?house
        a :House ;
        ?source ?id .
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("source", new Uri(BaseController.schema, source));
            query.SetLiteral("id", id);

            return BaseController.Execute(query);
        }

        // Ruby route:   get '/houses/:letters', to: 'houses#lookup_by_letters'
        [Route("{letters:alpha:minlength(2)}", Name = "HouseByLetters")]
        [HttpGet]
        public Graph ByLetters(string letters)
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?house 
        a :House;
        :houseName ?houseName .
}
WHERE {
    ?house a :House.
    ?house :houseName ?houseName .

    FILTER CONTAINS(LCASE(?houseName), LCASE(@letters))
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letters", letters);

            return BaseController.Execute(query);
        }
    }
}
