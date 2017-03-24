namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Net.Http;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("constituencies")]
    public class ConstituencyController : BaseController
    {
        // Ruby route: match '/constituencies/:constituency', to: 'constituencies#show', constituency: /\w{8}-\w{4}-\w{4}-\w{4}-\w{12}/, via: [:get]
        [Route("{id:guid}", Name = "ConstituencyByID")]
        [HttpGet]
        public HttpResponseMessage ById(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a parl:ConstituencyGroup ;
        parl:constituencyGroupEndDate ?endDate ;
        parl:constituencyGroupStartDate ?startDate ;
        parl:constituencyGroupName ?name ;
        parl:constituencyGroupOnsCode ?onsCode ;
        parl:constituencyGroupHasConstituencyArea ?constituencyArea .
    ?constituencyArea
        a parl:ConstituencyArea ;
        parl:constituencyAreaLatitude ?latitude ;
        parl:constituencyAreaLongitude ?longitude ;
        parl:constituencyAreaExtent ?polygon .
    ?constituencyGroup parl:constituencyGroupHasHouseSeat ?houseSeat .
    ?houseSeat a parl:HouseSeat ;
        parl:houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency a parl:SeatIncumbency ;
        parl:incumbencyHasMember ?member ;
        parl:incumbencyEndDate ?seatIncumbencyEndDate ;
        parl:incumbencyStartDate ?seatIncumbencyStartDate .
    ?member a parl:Person ;
        parl:personGivenName ?givenName ;
        parl:personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
}
WHERE {
    BIND( @id AS ?constituencyGroup )
        ?constituencyGroup a parl:ConstituencyGroup .
        OPTIONAL { ?constituencyGroup parl:constituencyGroupEndDate ?endDate . }
        OPTIONAL { ?constituencyGroup parl:constituencyGroupStartDate ?startDate . }
        OPTIONAL { ?constituencyGroup parl:constituencyGroupName ?name . }
        OPTIONAL { ?constituencyGroup parl:constituencyGroupOnsCode ?onsCode . }
        OPTIONAL {
        ?constituencyGroup parl:constituencyGroupHasConstituencyArea ?constituencyArea .
        ?constituencyArea a parl:ConstituencyArea .
        OPTIONAL { ?constituencyArea parl:constituencyAreaLatitude ?latitude . }
        OPTIONAL { ?constituencyArea parl:constituencyAreaLongitude ?longitude . }
        OPTIONAL { ?constituencyArea parl:constituencyAreaExtent ?polygon . }
}
        OPTIONAL {
        ?constituencyGroup parl:constituencyGroupHasHouseSeat ?houseSeat .
        ?houseSeat parl:houseSeatHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency a parl:SeatIncumbency ;
        OPTIONAL { ?seatIncumbency parl:incumbencyHasMember ?member . }
        OPTIONAL { ?seatIncumbency parl:incumbencyEndDate ?seatIncumbencyEndDate . }
        OPTIONAL { ?seatIncumbency parl:incumbencyStartDate ?seatIncumbencyStartDate . }
        OPTIONAL { ?member parl:personGivenName ?givenName . }
        OPTIONAL { ?member parl:personFamilyName ?familyName . }
        OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
}
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: match '/constituencies/:letter', to: 'constituencies#letters', letter: /[A-Za-z]/, via: [:get]
        [Route(@"{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ConstituencyByInitial")]
        [HttpGet]
        public HttpResponseMessage ByInitial(string initial)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a parl:ConstituencyGroup ;
        parl:constituencyGroupName ?name ;
        parl:constituencyGroupEndDate ?endDate .
}
WHERE {
    ?constituencyGroup a parl:ConstituencyGroup .
    OPTIONAL { ?constituencyGroup parl:constituencyGroupName ?name . }
    OPTIONAL { ?constituencyGroup parl:constituencyGroupEndDate ?endDate . }

    FILTER STRSTARTS(LCASE(?name), LCASE(@letter)) 
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letter", initial);

            return Execute(query);
        }

        // Ruby route: get '/constituencies/current', to: 'constituencies#current'
        [Route("current", Name = "ConstituencyCurrent")]
        [HttpGet]
        public HttpResponseMessage Current()
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a parl:ConstituencyGroup ;
        parl:constituencyGroupName ?name ;
        parl:constituencyGroupHasHouseSeat ?seat .
    ?seat
        a parl:HouseSeat ;
        parl:houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a parl:SeatIncumbency ;
        parl:incumbencyHasMember ?member .
    ?member
        a parl:Person ;
        parl:personGivenName ?givenName ;
        parl:personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
}
WHERE {
    ?constituencyGroup a parl:ConstituencyGroup .
    FILTER NOT EXISTS { ?constituencyGroup a parl:PastConstituencyGroup . }
    OPTIONAL { ?constituencyGroup parl:constituencyGroupName ?name . }
    OPTIONAL {
        ?constituencyGroup parl:constituencyGroupHasHouseSeat ?seat .
        ?seat parl:houseSeatHasSeatIncumbency ?seatIncumbency .
        FILTER NOT EXISTS { ?seatIncumbency a parl:PastIncumbency . }
        ?seatIncumbency parl:incumbencyHasMember ?member .
        OPTIONAL { ?member parl:personGivenName ?givenName . }
        OPTIONAL { ?member parl:personFamilyName ?familyName . }
        OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
    }
}
";

            return Execute(queryString);
        }

        // Ruby route: get '/constituencies/lookup', to: 'constituencies#lookup'
        [Route(@"lookup/{source:regex(^\p{L}+$)}/{id}", Name = "ConstituencyLookup")]
        [HttpGet]
        public HttpResponseMessage Lookup(string source, string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituency
        a parl:ConstituencyGroup .
}
WHERE {
    BIND(@id AS ?id)
        BIND(@source AS ?source)


        ?constituency a parl:ConstituencyGroup.
        ?constituency ?source ?id.
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("source", new Uri(BaseController.schema, source));
            query.SetLiteral("id", id);

            return Execute(query);
        }

        // Ruby route: get '/constituencies/:letters', to: 'constituencies#lookup_by_letters'
        // Was this not going to be called ByInitials? - CJA

        [Route(@"{letters:regex(^\p{L}+$):minlength(2)}", Name = "ConstituencyByLetters")]
        [HttpGet]
        public HttpResponseMessage ByLetters(string letters)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituency
        a parl:ConstituencyGroup ;
        parl:constituencyGroupName ?constituencyName ;
        parl:constituencyGroupEndDate ?endDate .
}
WHERE {
    ?constituency a parl:ConstituencyGroup .
    ?constituency parl:constituencyGroupName ?constituencyName .
    OPTIONAL { ?constituency parl:constituencyGroupEndDate ?endDate . }

    FILTER CONTAINS(LCASE(?constituencyName), LCASE(@letters))
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letters", letters);

            return Execute(query);
        }

        // Ruby route: get '/constituencies/a_z_letters', to: 'constituencies#a_z_letters'
        [Route("a_z_letters", Name = "ConstituencyAToZ")]
        [HttpGet]
        public HttpResponseMessage AToZLetters()
        {
            var queryString = @"

PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT {
    _:x parl:value ?firstLetter .
}
WHERE {
    SELECT DISTINCT ?firstLetter WHERE {
        ?s a parl:ConstituencyGroup .
        ?s parl:constituencyGroupName ?constituencyName .

        BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
    }
}
";
    
            var query = new SparqlParameterizedString(queryString);
            return Execute(query);
        }

        // Ruby route: match '/constituencies/current/:letter', to: 'constituencies#current_letters', letter: /[A-Za-z]/, via: [:get]
        [Route("current/{initial:maxlength(1)}", Name = "ConstituencyCurrentByInitial")]
        [HttpGet]
        public HttpResponseMessage CurrentByLetters(string initial)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a parl:ConstituencyGroup ;
        parl:constituencyGroupName ?name ;
        parl:constituencyGroupHasHouseSeat ?seat .
    ?seat
        a parl:HouseSeat ;
        parl:houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a parl:SeatIncumbency ;
        parl:incumbencyHasMember ?member .
    ?member
        a parl:Person ;
        parl:personGivenName ?givenName ;
        parl:personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
}
WHERE {
    ?constituencyGroup a parl:ConstituencyGroup .
    FILTER NOT EXISTS { ?constituencyGroup a parl:PastConstituencyGroup . }
    OPTIONAL { ?constituencyGroup parl:constituencyGroupName ?name . }
    OPTIONAL {
        ?constituencyGroup parl:constituencyGroupHasHouseSeat ?seat .
        ?seat parl:houseSeatHasSeatIncumbency ?seatIncumbency .
        FILTER NOT EXISTS { ?seatIncumbency a parl:PastIncumbency . }
        ?seatIncumbency parl:incumbencyHasMember ?member .
        OPTIONAL { ?member parl:personGivenName ?givenName . }
        OPTIONAL { ?member parl:personFamilyName ?familyName . }
        OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
    }

    FILTER STRSTARTS(LCASE(?name), LCASE(@initial))

}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return Execute(query);
        }
        // Ruby route: get '/constituencies/current/a_z_letters', to: 'constituencies#a_z_letters_current'
        [Route("current/a_z_letters", Name = "ConstituencyCurrentAToZ")]
        [HttpGet]
        public HttpResponseMessage CurrentAToZLetters()
        {
            var queryString = @"


PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT {
    _:x parl:value ?firstLetter .
}
WHERE {
    SELECT DISTINCT ?firstLetter WHERE {
        ?s a parl:ConstituencyGroup .
        FILTER NOT EXISTS { ?s a parl:PastConstituencyGroup . }
        ?s parl:constituencyGroupName ?constituencyName .

        BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return Execute(query);
        }

        // Ruby route: resources :constituencies, only: [:index]
        [Route("", Name = "ConstituencyIndex")]
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var queryString = @"


PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a parl:ConstituencyGroup ;
        parl:constituencyGroupName ?name ;
        parl:constituencyGroupEndDate ?endDate .
}
WHERE {
    ?constituencyGroup a parl:ConstituencyGroup .
    OPTIONAL { ?constituencyGroup parl:constituencyGroupName ?name . }
    OPTIONAL { ?constituencyGroup parl:constituencyGroupEndDate ?endDate . }

}
";

            var query = new SparqlParameterizedString(queryString);

            return Execute(query);
        }

        // Ruby route: resources :constituencies, only: [:index] do get '/members', to: 'constituencies#members' end
        [Route("{id:guid}/members", Name = "ConstituencyMembers")]
        [HttpGet]
        public HttpResponseMessage Members(string id)
        {
            var queryString = @"



PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a parl:ConstituencyGroup ;
        parl:constituencyGroupName ?name ;
        parl:constituencyGroupHasHouseSeat ?houseSeat ;
        parl:constituencyGroupStartDate ?constituencyGroupStartDate ;
        parl:constituencyGroupEndDate ?constituencyGroupEndDate .
    ?houseSeat a parl:HouseSeat ;
        parl:houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency a parl:SeatIncumbency ;
        parl:incumbencyHasMember ?member ;
        parl:incumbencyEndDate ?seatIncumbencyEndDate ;
        parl:incumbencyStartDate ?seatIncumbencyStartDate .
    ?member a parl:Person ;
        parl:personGivenName ?givenName ;
        parl:personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
}
WHERE {
    BIND( @constituencyid AS ?constituencyGroup )

        ?constituencyGroup a parl:ConstituencyGroup ;
        parl:constituencyGroupHasHouseSeat ?houseSeat .
        OPTIONAL { ?constituencyGroup parl:constituencyGroupName ?name . }
        OPTIONAL { ?constituencyGroup parl:constituencyGroupEndDate ?constituencyGroupEndDate . }
        OPTIONAL { ?constituencyGroup parl:constituencyGroupStartDate ?constituencyGroupStartDate . }
        OPTIONAL {
        ?houseSeat parl:houseSeatHasSeatIncumbency ?seatIncumbency .
        OPTIONAL {
        ?seatIncumbency parl:incumbencyHasMember ?member .
        OPTIONAL { ?seatIncumbency parl:incumbencyEndDate ?seatIncumbencyEndDate . }
        OPTIONAL { ?seatIncumbency parl:incumbencyStartDate ?seatIncumbencyStartDate . }
        OPTIONAL { ?member parl:personGivenName ?givenName . }
        OPTIONAL { ?member parl:personFamilyName ?familyName . }
        OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
}
}
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("constituencyid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :constituencies, only: [:index] do get '/members/current', to: 'constituencies#current_member' end
        [Route("{id:guid}/members/current", Name = "ConstituencyCurrentMember")]
        [HttpGet]
        public HttpResponseMessage CurrentMembers(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a parl:ConstituencyGroup ;
        parl:constituencyGroupName ?name ;
        parl:constituencyGroupStartDate ?constituencyGroupStartDate ;
        parl:constituencyGroupEndDate ?constituencyGroupEndDate ;
        parl:constituencyGroupHasHouseSeat ?houseSeat .
    ?houseSeat a parl:HouseSeat ;
        parl:houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency a parl:SeatIncumbency ;
        parl:incumbencyHasMember ?member ;
        parl:incumbencyEndDate ?seatIncumbencyEndDate ;
        parl:incumbencyStartDate ?seatIncumbencyStartDate .
    ?member a parl:Person ;
        parl:personGivenName ?givenName ;
        parl:personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
}
WHERE {
    BIND(@constituencyid AS ?constituencyGroup )

        ?constituencyGroup a parl:ConstituencyGroup ;
        parl:constituencyGroupHasHouseSeat ?houseSeat .
        OPTIONAL { ?constituencyGroup parl:constituencyGroupName ?name . }
        OPTIONAL { ?constituencyGroup parl:constituencyGroupStartDate ?constituencyGroupStartDate . }
        OPTIONAL { ?constituencyGroup parl:constituencyGroupEndDate ?constituencyGroupEndDate . }
        OPTIONAL {
        ?houseSeat parl:houseSeatHasSeatIncumbency ?seatIncumbency .
        FILTER NOT EXISTS { ?seatIncumbency a parl:PastIncumbency . }
        OPTIONAL {
        ?seatIncumbency parl:incumbencyHasMember ?member .
        OPTIONAL { ?seatIncumbency parl:incumbencyEndDate ?seatIncumbencyEndDate . }
        OPTIONAL { ?seatIncumbency parl:incumbencyStartDate ?seatIncumbencyStartDate . }
        OPTIONAL { ?member parl:personGivenName ?givenName . }
        OPTIONAL { ?member parl:personFamilyName ?familyName . }
        OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
}
}
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("constituencyid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :constituencies, only: [:index] do get '/contact_point', to: 'constituencies#contact_point' end
        // why is this singular?
        [Route("{id:guid}/contact_point", Name = "ConstituencyContactPoint")]
        [HttpGet]
        public HttpResponseMessage ContactPoint(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituencyGroup a parl:ConstituencyGroup ;
        parl:constituencyGroupHasHouseSeat ?houseSeat ;
        parl:constituencyGroupName ?name .
    ?houseSeat a parl:HouseSeat ;
        parl:houseSeatHasSeatIncumbency ?incumbency .
    ?incumbency a parl:SeatIncumbency ;
        parl:incumbencyHasContactPoint ?contactPoint .
    ?contactPoint a parl:ContactPoint ;
        parl:email ?email ;
        parl:phoneNumber ?phoneNumber ;
        parl:faxNumber ?faxNumber ;
        parl:contactForm ?contactForm ;
        parl:contactPointHasPostalAddress ?postalAddress .
    ?postalAddress a parl:PostalAddress ;
        parl:postCode ?postCode ;
        parl:addressLine1 ?addressLine1 ;
        parl:addressLine2 ?addressLine2 ;
        parl:addressLine3 ?addressLine3 ;
        parl:addressLine4 ?addressLine4 ;
        parl:addressLine5 ?addressLine5 .
}
WHERE {
    BIND(@constituencyid AS ?constituencyGroup )

        ?constituencyGroup a parl:ConstituencyGroup .
        OPTIONAL {
        ?constituencyGroup parl:constituencyGroupHasHouseSeat ?houseSeat .
        OPTIONAL {
        ?houseSeat parl:houseSeatHasSeatIncumbency ?incumbency .
        FILTER NOT EXISTS { ?incumbency a parl:PastIncumbency . }
        OPTIONAL {
        ?incumbency parl:incumbencyHasContactPoint ?contactPoint .
        OPTIONAL{ ?contactPoint parl:email ?email . }
        OPTIONAL{ ?contactPoint parl:phoneNumber ?phoneNumber . }
        OPTIONAL{ ?contactPoint parl:faxNumber ?faxNumber . }
        OPTIONAL{ ?contactPoint parl:contactForm ?contactForm . }
        OPTIONAL{ ?contactPoint parl:contactPointHasPostalAddress ?postalAddress .
        OPTIONAL{ ?postalAddress parl:postCode ?postCode . }
        OPTIONAL{ ?postalAddress parl:addressLine1 ?addressLine1 . }
        OPTIONAL{ ?postalAddress parl:addressLine2 ?addressLine2 . }
        OPTIONAL{ ?postalAddress parl:addressLine3 ?addressLine3 . }
        OPTIONAL{ ?postalAddress parl:addressLine4 ?addressLine4 . }
        OPTIONAL{ ?postalAddress parl:addressLine5 ?addressLine5 . }
}
}
}
}
        OPTIONAL { ?constituencyGroup parl:constituencyGroupName ?name . }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("constituencyid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

    }
}
