namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("people/members")]
    public class MemberController : BaseController
    {
        // Ruby route: get '/people/members/current', to: 'members#current'
        [Route("current", Name = "MemberCurrent")]
        [HttpGet]
        public Graph Current()
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?seatIncumbency
        a parl:SeatIncumbency ;
        parl:seatIncumbencyHasHouseSeat ?houseSeat .
    ?member
        a parl:Person ;
        parl:personGivenName ?givenName ;
        parl:personFamilyName ?familyName ;
        parl:partyMemberHasPartyMembership ?partyMembership ;
        parl:memberHasSeatIncumbency ?seatIncumbency .
    ?partyMembership
        a parl:PartyMembership ;
        parl:partyMembershipHasParty ?party .
    ?party
        a parl:Party ;
        parl:partyName ?partyName .
    ?houseSeat
        a parl:HouseSeat ;
        parl:houseSeatHasHouse ?house ;
        parl:houseSeatHasConstituencyGroup ?constituencyGroup .
    ?house
        a parl:House ;
        parl:houseName ?houseName .
    ?constituencyGroup
        a parl:ConstituencyGroup ;
        parl:constituencyGroupName ?constituencyGroupName .
}
WHERE {
    ?seatIncumbency a parl:SeatIncumbency ;
    FILTER NOT EXISTS { ?seatIncumbency a parl:PastSeatIncumbency .	}
    ?seatIncumbency
        parl:seatIncumbencyHasMember ?member ;
        parl:seatIncumbencyHasHouseSeat ?houseSeat .
    ?member parl:partyMemberHasPartyMembership ?partyMembership .
    FILTER NOT EXISTS { ?partyMembership a parl:PastPartyMembership . }
    ?partyMembership parl:partyMembershipHasParty ?party .
    ?party
        parl:partyName ?partyName .
        ?houseSeat parl:houseSeatHasHouse ?house ;
        parl:houseSeatHasConstituencyGroup ?constituencyGroup .
    ?constituencyGroup parl:constituencyGroupName ?constituencyGroupName .
    ?house parl:houseName ?houseName .
    OPTIONAL { ?member parl:personGivenName ?givenName . }
    OPTIONAL { ?member parl:personFamilyName ?familyName . }
}";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.Execute(query);
        }

        // Ruby route: match '/people/members/:letter', to: 'members#letters', letter: /[A-Za-z]/, via: [:get]
        // TODO: {x:alpha}?
        // TODO: {x:regex} with unicode alpha?
        // TODO: accents?
        [Route("{initial:maxlength(1)}", Name = "MemberByInitial")]
        [HttpGet]
        public Graph ByInitial(string initial)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?seatIncumbency
        a parl:SeatIncumbency ;
        parl:seatIncumbencyHasMember ?member ;
        parl:seatIncumbencyHasHouseSeat ?houseSeat .
    ?member
        a parl:Person ;
        parl:personGivenName ?givenName ;
        parl:personFamilyName ?familyName ;
        parl:partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership
        a parl:PartyMembership ;
        parl:partyMembershipHasParty ?party .
    ?party
        a parl:Party ;
        parl:partyName ?partyName .
    ?houseSeat
        a parl:HouseSeat ;
        parl:houseSeatHasHouse ?house ;
        parl:houseSeatHasConstituencyGroup ?constituencyGroup .
    ?house
        a parl:House ;
        parl:houseName ?houseName .
    ?constituencyGroup
        a parl:ConstituencyGroup ;
        parl:constituencyGroupName ?constituencyGroupName .
}
WHERE {
    ?seatIncumbency
        a parl:SeatIncumbency ;
        parl:seatIncumbencyHasMember ?member ;
        parl:seatIncumbencyHasHouseSeat ?houseSeat .
    ?member parl:partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership parl:partyMembershipHasParty ?party .
    ?party parl:partyName ?partyName .
    ?houseSeat
        parl:houseSeatHasHouse ?house ;
        parl:houseSeatHasConstituencyGroup ?constituencyGroup .
    ?constituencyGroup parl:constituencyGroupName ?constituencyGroupName .
    ?house parl:houseName ?houseName .
    OPTIONAL { ?member parl:personGivenName ?givenName . }
    OPTIONAL { ?member parl:personFamilyName ?familyName . }

    FILTER STRSTARTS(LCASE(?familyName), LCASE(@letter))
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letter", initial);

            return BaseController.Execute(query);
        }

        // Ruby route: match '/people/members/current/:letter', to: 'members#current_letters', letter: /[A-Za-z]/, via: [:get]
        // TODO: {x:alpha}?
        // TODO: {x:regex} with unicode alpha?
        // TODO: accents?
        [Route("current/{initial:maxlength(1)}", Name = "MemberCurrentByInitial")]
        [HttpGet]
        public Graph CurrentByInitial(string initial)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>

CONSTRUCT{
    ?seatIncumbency
        a parl:SeatIncumbency ;
        parl:seatIncumbencyHasHouseSeat ?houseSeat .
    ?member
        a parl:Person ;
        parl:personGivenName ?givenName ;
        parl:personFamilyName ?familyName ;
        parl:partyMemberHasPartyMembership ?partyMembership ;
        parl:memberHasSeatIncumbency ?seatIncumbency .
    ?partyMembership
        a parl:PartyMembership ;
        parl:partyMembershipHasParty ?party .
    ?party
        a parl:Party ;
        parl:partyName ?partyName .
    ?houseSeat
        a parl:HouseSeat ;
        parl:houseSeatHasHouse ?house ;
        parl:houseSeatHasConstituencyGroup ?constituencyGroup .
    ?house
        a parl:House ;
        parl:houseName ?houseName .
    ?constituencyGroup
        a parl:ConstituencyGroup ;
        parl:constituencyGroupName ?constituencyGroupName .
}
WHERE {
    ?seatIncumbency a parl:SeatIncumbency .
    FILTER NOT EXISTS { ?seatIncumbency a parl:PastSeatIncumbency .	}
    ?seatIncumbency
        parl:seatIncumbencyHasMember ?member ;
        parl:seatIncumbencyHasHouseSeat ?houseSeat .
    ?member parl:partyMemberHasPartyMembership ?partyMembership .
    FILTER NOT EXISTS { ?partyMembership a parl:PastPartyMembership . }
    ?partyMembership parl:partyMembershipHasParty ?party .
    ?party parl:partyName ?partyName .
    ?houseSeat
        parl:houseSeatHasHouse ?house ;
        parl:houseSeatHasConstituencyGroup ?constituencyGroup .
    ?constituencyGroup parl:constituencyGroupName ?constituencyGroupName .
    ?house parl:houseName ?houseName .
    OPTIONAL { ?member parl:personGivenName ?givenName . }
    OPTIONAL { ?member parl:personFamilyName ?familyName . }
    FILTER STRSTARTS(LCASE(?familyName), LCASE(@letter)) .
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letter", initial);

            return BaseController.Execute(query);
        }
            

    }
}