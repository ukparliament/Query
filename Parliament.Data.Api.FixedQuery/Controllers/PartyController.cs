namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("parties")]
    public class PartyController : BaseController
    {
        // Ruby route: match '/parties/:party', to: 'parties#show', party: /\w{8}-\w{4}-\w{4}-\w{4}-\w{12}/, via: [:get]
        [Route("{id:guid}", Name = "PartyById")]
        [HttpGet]
        public Graph ById(string id)
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?name .
}
WHERE {
    BIND(@id AS ?party)

    ?party :partyName ?name .
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, id));

            return BaseController.Execute(query);

        }

        // Ruby route: get '/parties/:letter', to: 'parties#letters', letter: /[A-Za-z]/, via: [:get]
        [Route("{initial:alpha:maxlength(1)}", Name = "PartyByInitial")]
        [HttpGet]
        public Graph ByInitial(string initial)
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName .
}
WHERE {
    ?party
        a :Party ;
        :partyName ?partyName .

    FILTER STRSTARTS(LCASE(?partyName), LCASE(@letter)) .
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letter", initial);

            return BaseController.Execute(query);
        }

        // Ruby route: get '/parties/current', to: 'parties#current'
        [Route("current", Name = "PartyCurrent")]
        [HttpGet]
        public Graph Current() {
            var querystring = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName .
}
WHERE {
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasMember ?person .
    FILTER NOT EXISTS { ?seatIncumbency a :PastSeatIncumbency . }
    ?person :partyMemberHasPartyMembership ?partyMembership .
    FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
    ?partyMembership :partyMembershipHasParty ?party .
    ?party :partyName ?partyName .
}";

            return BaseController.Execute(querystring);
        }

        // Ruby route: get '/parties/a_z_letters', to: 'parties#a_z_letters_all'
        [Route("a-z", Name = "PartyAToZ")]
        [HttpGet]
        public Graph AToZLetters()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
     _:x :value ?firstLetter.
}
WHERE {
    SELECT DISTINCT ?firstLetter WHERE {
    ?party :partyName ?partyName .
    BIND(ucase(SUBSTR(?partyName, 1, 1)) as ?firstLetter)
    }
}";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.Execute(query);
        }

        // Ruby route: get '/parties/lookup', to: 'parties#lookup'
        [Route("lookup/{source:alpha}/{id}", Name = "PartyLookup")]
        [HttpGet]
        public Graph Lookup(string source, string id)
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party a :Party .
}
WHERE {
    BIND(@id AS ?id)
    BIND(@source AS ?source)

    ?party
        a :Party ;
        ?source ?id .
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("source", new Uri(BaseController.schema, source));
            query.SetLiteral("id", id);

            return BaseController.Execute(query);
        }
        // Ruby route: get '/parties/:letters', to: 'parties#lookup_by_letters'
        [Route("{letters:alpha:minlength(2)}", Name = "PartyByLetters")]
        [HttpGet]
        public Graph ByLetters(string letters)
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party 
        a :Party;
        :partyName ?partyName .
}
WHERE {
    ?party a :Party.
    ?party :partyName ?partyName .

    FILTER CONTAINS(LCASE(?partyName), LCASE(@letters))
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letters", letters);

            return BaseController.Execute(query);
        }
    }
}