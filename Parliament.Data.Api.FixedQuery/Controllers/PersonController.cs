namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("people")]
    public class PersonController : BaseController
    {
        // Ruby route: match '/people/:person', to: 'people#show', person: /\w{8}-\w{4}-\w{4}-\w{4}-\w{12}/, via: [:get]
        [Route("{id:guid}", Name = "PersonById")]
        [HttpGet]
        public Graph ById(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?person
        a parl:Person ;
        parl:personDateOfBirth ?dateOfBirth ;
        parl:personGivenName ?givenName ;
        parl:personOtherNames ?otherName ;
        parl:personFamilyName ?familyName ;
        parl:personHasGenderIdentity ?genderIdentity ;
        parl:partyMemberHasPartyMembership ?partyMembership ;
        parl:personHasContactPoint ?contactPoint ;
        parl:memberHasSeatIncumbency ?seatIncumbency .
    ?genderIdentity
        a parl:GenderIdentity ;
        parl:genderIdentityHasGender ?gender .
    ?gender
        a parl:Gender ;
        parl:genderName ?genderName .
    ?contactPoint
        a parl:ContactPoint ;
        parl:email ?email ;
        parl:phoneNumber ?phoneNumber ;
        parl:faxNumber ?faxNumber ;
        parl:contactPointHasPostalAddress ?postalAddress .
    ?postalAddress
        a parl:PostalAddress ;
        parl:addressLine1 ?addressLine1 ;
        parl:addressLine2 ?addressLine2 ;
        parl:addressLine3 ?addressLine3 ;
        parl:addressLine4 ?addressLine4 ;
        parl:addressLine5 ?addressLine5 ;
        parl:postCode ?postCode .
    ?constituency
        a parl:ConstituencyGroup ;
        parl:constituencyGroupName ?constituencyName .
    ?seatIncumbency
        a parl:SeatIncumbency ;
        parl:seatIncumbencyEndDate ?seatIncumbencyEndDate ;
        parl:seatIncumbencyStartDate ?seatIncumbencyStartDate ;
        parl:seatIncumbencyHasHouseSeat ?seat .
    ?seat
        a parl:HouseSeat ;
        parl:houseSeatHasConstituencyGroup ?constituency ;
        parl:houseSeatHasHouse ?house .
    ?party
        a parl:Party ;
        parl:partyName ?partyName .
    ?partyMembership
        a parl:PartyMembership ;
        parl:partyMembershipStartDate ?partyMembershipStartDate ;
        parl:partyMembershipEndDate ?partyMembershipEndDate ;
        parl:partyMembershipHasParty ?party .
        ?house a parl:House ;
        parl:houseName ?houseName .
}
WHERE {
    BIND(@id AS ?person)

    ?person
        a parl:Person ;
        parl:personHasGenderIdentity ?genderIdentity .
    OPTIONAL { ?person parl:personGivenName ?givenName . }
    OPTIONAL { ?person parl:personOtherNames ?otherName . }
    OPTIONAL { ?person parl:personFamilyName ?familyName . }
    OPTIONAL { ?person parl:personDateOfBirth ?dateOfBirth . }
    ?genderIdentity parl:genderIdentityHasGender ?gender .
    OPTIONAL { ?gender parl:genderName ?genderName . }
    OPTIONAL {
        ?person parl:memberHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency
            parl:seatIncumbencyHasHouseSeat ?seat ;
            parl:seatIncumbencyStartDate ?seatIncumbencyStartDate .
        ?seat
            parl:houseSeatHasConstituencyGroup ?constituency ;
            parl:houseSeatHasHouse ?house .
        OPTIONAL { ?seatIncumbency parl:seatIncumbencyEndDate ?seatIncumbencyEndDate . }
        ?constituency parl:constituencyGroupName ?constituencyName .
        ?house parl:houseName ?houseName .
    }
    OPTIONAL {
        ?person parl:partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership
            parl:partyMembershipHasParty ?party ;
            parl:partyMembershipStartDate ?partyMembershipStartDate .
        OPTIONAL { ?partyMembership parl:partyMembershipEndDate ?partyMembershipEndDate . }
        ?party parl:partyName ?partyName .
    }
    OPTIONAL {
        ?person parl:personHasContactPoint ?contactPoint .
        OPTIONAL { ?contactPoint parl:phoneNumber ?phoneNumber . }
        OPTIONAL { ?contactPoint parl:email ?email . }
        OPTIONAL { ?contactPoint parl:faxNumber ?faxNumber . }
        OPTIONAL {
        ?contactPoint parl:contactPointHasPostalAddress ?postalAddress .
        OPTIONAL { ?postalAddress parl:addressLine1 ?addressLine1 . }
        OPTIONAL { ?postalAddress parl:addressLine2 ?addressLine2 . }
        OPTIONAL { ?postalAddress parl:addressLine3 ?addressLine3 . }
        OPTIONAL { ?postalAddress parl:addressLine4 ?addressLine4 . }
        OPTIONAL { ?postalAddress parl:addressLine5 ?addressLine5 . }
        OPTIONAL { ?postalAddress parl:postCode ?postCode . }
        }
    }
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(instance, id));

            return BaseController.Execute(query);
        }

        // Ruby route: get '/people/:letter', to: 'people#letters', letter: /[A-Za-z]/, via: [:get]
        // TODO: accents ({x:regex} with unicode alpha)?
        // TODO: REGEX ignore case?
        [Route("{initial:maxlength(1)}", Name = "PersonByInitial")]
        [HttpGet]
        public Graph ByInitial(string initial)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
    CONSTRUCT {
    ?person
        a parl:Person ;
        parl:personGivenName ?givenName ;
        parl:personFamilyName ?familyName .
}
WHERE {
    ?person
        a parl:Person ;
        parl:personFamilyName ?familyName .
    OPTIONAL { ?person parl:personGivenName ?givenName } .

    FILTER STRSTARTS(LCASE(?familyName), LCASE(@letter))
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letter", initial);

            return BaseController.Execute(query);

        }

        // Ruby route: get '/people/lookup', to: 'people#lookup'
        // TODO: validate source against actual properties?
        // TODO: validate cource and id combnation?
        // TODO: source could have numbers?
        [Route("lookup/{source:alpha}/{id}", Name = "PersonLookup")]
        [HttpGet]
        public Graph ByExternalIdentifier(string source, string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?person a parl:Person .
}
WHERE {
    BIND(@id AS ?id)
    BIND(@source AS ?source)

    ?person
        a parl:Person ;
        ?source ?id .
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("source", new Uri(BaseController.schema, source));
            query.SetLiteral("id", id);

            return BaseController.Execute(query);
        }

        // Ruby: get '/people/members', to: 'members#index'
        [Route("members", Name = "Member")]
        [HttpGet]
        public Graph Member()
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
    ?seatIncumbency a parl:SeatIncumbency ;
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
}";

            return BaseController.Execute(queryString);
        }

        // Ruby route: get '/people/:letters', to: 'people#lookup_by_letters'
        // TODO: letters length?
        // TODO: letters should be in query string?
        // TODO: STR required because OPTIONAL?
        // TODO: accents?
        // TODO: could be CONTAINS?
        // TODO: letters go in STR?
        [Route("{letters:alpha:minlength(2)}", Name = "PersonByLetters")]
        [HttpGet]
        public Graph ByLetters(string letters)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?person
        a parl:Person ;
        parl:personGivenName ?givenName ;
        parl:personFamilyName ?familyName .
}
WHERE {
    ?person a parl:Person .
    OPTIONAL { ?person parl:personGivenName ?givenName . }
    OPTIONAL { ?person parl:personFamilyName ?familyName . }

    FILTER (REGEX(STR(?familyName), @letters, 'i') || REGEX(STR(?givenName), @letters, 'i'))
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letters", letters);

            return BaseController.Execute(query);
        }
    }
}