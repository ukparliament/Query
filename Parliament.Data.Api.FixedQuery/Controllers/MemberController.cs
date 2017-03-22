namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Net.Http;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("people/members")]
    public class MemberController : BaseController
    {
        // Ruby route: get '/people/members/current', to: 'members#current'
        [Route("current", Name = "MemberCurrent")]
        [HttpGet]
        public HttpResponseMessage Current()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasIncumbency ?seatIncumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?seatIncumbencyStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
 	?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house .
    ?house
        a :House ;
        :houseName ?houseName .
    ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?partyMembershipStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
    }
WHERE {
  	?person a :Person ;
    :memberHasIncumbency ?incumbency .
    OPTIONAL { ?person :personGivenName ?givenName . }
    OPTIONAL { ?person :personFamilyName ?familyName . }
    { ?incumbency a :HouseIncumbency .
        BIND(?incumbency AS ?houseIncumbency)
        ?houseIncumbency :houseIncumbencyHasHouse ?house .
        ?house :houseName ?houseName .
    }
UNION {
    ?incumbency a :SeatIncumbency .
    BIND(?incumbency AS ?seatIncumbency)
    ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
    ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate .
    OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
    ?houseSeat :houseSeatHasHouse ?house .
    ?house :houseName ?houseName .
    OPTIONAL {?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
        ?constituencyGroup :constituencyGroupName ?constituencyName . 
        }
    }
    ?person :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership :partyMembershipHasParty ?party .
    ?party :partyName ?partyName .
    ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .
    OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
    FILTER NOT EXISTS {?incumbency a :PastIncumbency}
}
";

            var query = new SparqlParameterizedString(queryString);

            return Execute(query);
        }

        // Ruby route: match '/people/members/:letter', to: 'members#letters', letter: /[A-Za-z]/, via: [:get]
        // TODO: {x:regex(^\p{L}+$)}?
        // TODO: {x:regex} with unicode alpha?
        // TODO: accents?
        [Route("{initial:maxlength(1)}", Name = "MemberByInitial")]
        [HttpGet]
        public HttpResponseMessage ByInitial(string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasIncumbency ?seatIncumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?seatIncumbencyStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
   	?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house .
    ?house
        a :House ;
        :houseName ?houseName .
    ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?partyMembershipStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
    }
WHERE {
  	?person a :Person ;
    :memberHasIncumbency ?incumbency .
    OPTIONAL { ?person :personGivenName ?givenName . }
    OPTIONAL { ?person :personFamilyName ?familyName . }
    { ?incumbency a :HouseIncumbency .
        BIND(?incumbency AS ?houseIncumbency)
        ?houseIncumbency :houseIncumbencyHasHouse ?house .
        ?house :houseName ?houseName .
    }
    UNION {
    ?incumbency a :SeatIncumbency .
        BIND(?incumbency AS ?seatIncumbency)
        ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
        ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate .
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        ?houseSeat :houseSeatHasHouse ?house .
        ?house :houseName ?houseName .
    	OPTIONAL {?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
            ?constituencyGroup :constituencyGroupName ?constituencyName . 
        }
   	}
    ?person :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership :partyMembershipHasParty ?party .
    ?party :partyName ?partyName .
    ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .
    OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
    FILTER STRSTARTS(LCASE(?familyName), LCASE(@letter))
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letter", initial);

            return Execute(query);
        }

        // Ruby route: get '/people/members/a_z_letters', to: 'members#a_z_letters'
        [Route("a_z_letters", Name = "MemberAToZ")]
        [HttpGet]
        public HttpResponseMessage AToZLetters()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
     _:x :value ?firstLetter.
}
WHERE {
    SELECT DISTINCT ?firstLetter WHERE {
    ?incumbency :incumbencyHasMember ?member.
    ?member :personFamilyName ?familyName .

    BIND(ucase(SUBSTR(?familyName, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return Execute(query);
        }
    




        // Ruby route: match '/people/members/current/:letter', to: 'members#current_letters', letter: /[A-Za-z]/, via: [:get]
        // TODO: {x:regex(^\p{L}+$)}?
        // TODO: {x:regex} with unicode alpha?
        // TODO: accents?
        [Route("current/{initial:maxlength(1)}", Name = "MemberCurrentByInitial")]
        [HttpGet]
        public HttpResponseMessage CurrentByInitial(string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasIncumbency ?seatIncumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?seatIncumbencyStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
 	?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house .
    ?house
        a :House ;
        :houseName ?houseName .
    ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?partyMembershipStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
    }
WHERE {
  	?person 
        a :Person ;
        :memberHasIncumbency ?incumbency .
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        { ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency)
            ?houseIncumbency :houseIncumbencyHasHouse ?house .
            ?house :houseName ?houseName .
   		}
        UNION {
            ?incumbency a :SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency)
            ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
            ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        	?houseSeat :houseSeatHasHouse ?house .
            ?house :houseName ?houseName .
    		OPTIONAL {?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
        		?constituencyGroup :constituencyGroupName ?constituencyName . 
            }
   		}
  	?person :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership :partyMembershipHasParty ?party .
    ?party :partyName ?partyName .
    ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .
    OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
    FILTER NOT EXISTS {?incumbency a :PastIncumbency}
    FILTER STRSTARTS(LCASE(?familyName), LCASE(@letter)) .
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letter", initial);

            return Execute(query);
        }

        // Ruby route: get '/people/members/current/a_z_letters', to: 'members#a_z_letters_current'
        [Route("current/a_z_letters", Name = "MemberCurrentAToZ")]
        [HttpGet]
        public HttpResponseMessage CurrentAToZLetters()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
     _:x :value ?firstLetter.
}
WHERE {
    SELECT DISTINCT ?firstLetter WHERE {
    ?incumbency :incumbencyHasMember ?member.
    FILTER NOT EXISTS {?incumbency a :PastIncumbency. }
    ?member :personFamilyName ?familyName .

    BIND(ucase(SUBSTR(?familyName, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return Execute(query);
        }

    }
}

