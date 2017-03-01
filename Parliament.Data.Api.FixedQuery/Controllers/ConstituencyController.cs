namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("constituencies")]
    public class ConstituencyController : BaseController
    {
        // Ruby route: '/constituencies/:constituency', to: 'constituencies#show', constituency: /\w{8}-\w{4}-\w{4}-\w{4}-\w{12}/, via: [:get]
        [Route("{id:guid}", Name = "ConstituencyByID")]
        [HttpGet]
        public Graph ById(string id)
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupEndDate ?endDate ;
        :constituencyGroupStartDate ?startDate ;
        :constituencyGroupName ?name ;
        :constituencyGroupOnsCode ?onsCode ;
        :constituencyGroupHasConstituencyArea ?constituencyArea ;
        :constituencyGroupHasHouseSeat ?houseSeat .
    ?constituencyArea
        a :ConstituencyArea ;
        :constituencyAreaLatitude ?latitude ;
        :constituencyAreaLongitude ?longitude ;
        :constituencyAreaExtent ?polygon .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasMember ?member ;
        :seatIncumbencyEndDate ?seatIncumbencyEndDate ;
        :seatIncumbencyStartDate ?seatIncumbencyStartDate .
    ?member
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName .
}
WHERE {
    BIND (@id AS ?constituencyGroup)

    ?constituencyGroup a :ConstituencyGroup .
    OPTIONAL { ?constituencyGroup :constituencyGroupEndDate ?endDate . }
    OPTIONAL { ?constituencyGroup :constituencyGroupStartDate ?startDate . }
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
    OPTIONAL { ?constituencyGroup :constituencyGroupOnsCode ?onsCode . }
    OPTIONAL {
        ?constituencyGroup :constituencyGroupHasConstituencyArea ?constituencyArea .
        ?constituencyArea a :ConstituencyArea .
        OPTIONAL { ?constituencyArea :constituencyAreaLatitude ?latitude . }
        OPTIONAL { ?constituencyArea :constituencyAreaLongitude ?longitude . }
        OPTIONAL { ?constituencyArea :constituencyAreaExtent ?polygon . }
    }
    OPTIONAL {
        ?constituencyGroup :constituencyGroupHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency a :SeatIncumbency .
        OPTIONAL { ?seatIncumbency :seatIncumbencyHasMember ?member . }
        OPTIONAL { ?seatIncumbency :seatIncumbencyEndDate ?seatIncumbencyEndDate . }
        OPTIONAL { ?seatIncumbency :seatIncumbencyStartDate ?seatIncumbencyStartDate . }
        OPTIONAL { ?member :personGivenName ?givenName . }
        OPTIONAL { ?member :personFamilyName ?familyName . }
    }
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, id));

            return BaseController.Execute(query);
        }

        // Ruby route: match '/constituencies/:letter', to: 'constituencies#letters', letter: /[A-Za-z]/, via: [:get]
        [Route("{initial:alpha:maxlength(1)}", Name = "ConstituencyByInitial")]
        [HttpGet]
        public Graph ByInitial(string initial)
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name .
}
WHERE {
    ?constituencyGroup a :ConstituencyGroup .
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }

    FILTER STRSTARTS(LCASE(?name), LCASE(@letter)) 
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letter", initial);

            return BaseController.Execute(query);
        }

        // Ruby route: get '/constituencies/current', to: 'constituencies#current'
        [Route("current", Name = "ConstituencyCurrent")]
        [HttpGet]
        public Graph Current()
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name .
}
WHERE {
    ?constituencyGroup a :ConstituencyGroup .
    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
}";

            return BaseController.Execute(queryString);
        }

        // Ruby route: get '/constituencies/lookup', to: 'constituencies#lookup'
        [Route("lookup/{source:alpha}/{id}", Name = "ConstituencyLookup")]
        [HttpGet]
        public Graph Lookup(string source, string id)
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituency a :ConstituencyGroup .
}
WHERE {
    BIND(@id AS ?id)
    BIND(@source AS ?source)

    ?constituency
        a :ConstituencyGroup ;
        ?source ?id .
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("source", new Uri(BaseController.schema, source));
            query.SetLiteral("id", id);

            return BaseController.Execute(query);
        }

        // Ruby route: get '/constituencies/:letters', to: 'constituencies#lookup_by_letters'
        // Was this not going to be called ByInitials? - CJA
        [Route("{letters:alpha:minlength(2)}", Name = "ConstituencyByLetters")]
        [HttpGet]
        public Graph ByLetters(string letters)
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituency
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
}
WHERE {
    ?constituency
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .

    FILTER CONTAINS(LCASE(?constituencyName), LCASE(@letters))
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letters", letters);

            return BaseController.Execute(query);
        }

        // Ruby route: match '/constituencies/current/:letter', to: 'constituencies#current_letters', letter: /[A-Za-z]/, via: [:get]
        [Route("current/{initial:maxlength(1)}", Name = "ConstituencyCurrentByInitial")]
        [HttpGet]
        public Graph CurrentByLetters(string initial)
        {
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name .
}
WHERE {
    ?constituencyGroup a :ConstituencyGroup .
    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
    FILTER STRSTARTS(LCASE(?name), LCASE(@initial))
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.Execute(query);
        }
    }
}