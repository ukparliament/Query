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
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?person
        a :Person ;
        :personDateOfBirth ?dateOfBirth ;
        :personGivenName ?givenName ;
        :personOtherNames ?otherName ;
        :personFamilyName ?familyName ;
        :personHasGenderIdentity ?genderIdentity ;
        :partyMemberHasPartyMembership ?partyMembership ;
        :personHasContactPoint ?contactPoint ;
        :memberHasSeatIncumbency ?seatIncumbency .
    ?genderIdentity
        a :GenderIdentity ;
        :genderIdentityHasGender ?gender .
    ?gender
        a :Gender ;
        :genderName ?genderName .
    ?contactPoint
        a :ContactPoint ;
        :email ?email ;
        :phoneNumber ?phoneNumber ;
        :faxNumber ?faxNumber ;
        :contactPointHasPostalAddress ?postalAddress .
    ?postalAddress
        a :PostalAddress ;
        :addressLine1 ?addressLine1 ;
        :addressLine2 ?addressLine2 ;
        :addressLine3 ?addressLine3 ;
        :addressLine4 ?addressLine4 ;
        :addressLine5 ?addressLine5 ;
        :postCode ?postCode .
    ?constituency
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyEndDate ?seatIncumbencyEndDate ;
        :seatIncumbencyStartDate ?seatIncumbencyStartDate ;
        :seatIncumbencyHasHouseSeat ?seat .
    ?seat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituency ;
        :houseSeatHasHouse ?house .
    ?party
        a :Party ;
        :partyName ?partyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipStartDate ?partyMembershipStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate ;
        :partyMembershipHasParty ?party .
        ?house a :House ;
        :houseName ?houseName .
}
WHERE {
    BIND(@id AS ?person)

    ?person
        a :Person ;
        :personHasGenderIdentity ?genderIdentity .
    OPTIONAL { ?person :personGivenName ?givenName . }
    OPTIONAL { ?person :personOtherNames ?otherName . }
    OPTIONAL { ?person :personFamilyName ?familyName . }
    OPTIONAL { ?person :personDateOfBirth ?dateOfBirth . }
    ?genderIdentity :genderIdentityHasGender ?gender .
    OPTIONAL { ?gender :genderName ?genderName . }
    OPTIONAL {
        ?person :memberHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency
            :seatIncumbencyHasHouseSeat ?seat ;
            :seatIncumbencyStartDate ?seatIncumbencyStartDate .
        ?seat
            :houseSeatHasConstituencyGroup ?constituency ;
            :houseSeatHasHouse ?house .
        OPTIONAL { ?seatIncumbency :seatIncumbencyEndDate ?seatIncumbencyEndDate . }
        ?constituency :constituencyGroupName ?constituencyName .
        ?house :houseName ?houseName .
    }
    OPTIONAL {
        ?person :partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership
            :partyMembershipHasParty ?party ;
            :partyMembershipStartDate ?partyMembershipStartDate .
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
        ?party :partyName ?partyName .
    }
    OPTIONAL {
        ?person :personHasContactPoint ?contactPoint .
        OPTIONAL { ?contactPoint :phoneNumber ?phoneNumber . }
        OPTIONAL { ?contactPoint :email ?email . }
        OPTIONAL { ?contactPoint :faxNumber ?faxNumber . }
        OPTIONAL {
        ?contactPoint :contactPointHasPostalAddress ?postalAddress .
        OPTIONAL { ?postalAddress :addressLine1 ?addressLine1 . }
        OPTIONAL { ?postalAddress :addressLine2 ?addressLine2 . }
        OPTIONAL { ?postalAddress :addressLine3 ?addressLine3 . }
        OPTIONAL { ?postalAddress :addressLine4 ?addressLine4 . }
        OPTIONAL { ?postalAddress :addressLine5 ?addressLine5 . }
        OPTIONAL { ?postalAddress :postCode ?postCode . }
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
PREFIX : <http://id.ukpds.org/schema/>
    CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName .
}
WHERE {
    ?person
        a :Person ;
        :personFamilyName ?familyName .
    OPTIONAL { ?person :personGivenName ?givenName } .

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
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?person a :Person .
}
WHERE {
    BIND(@id AS ?id)
    BIND(@source AS ?source)

    ?person
        a :Person ;
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
            var queryString = @"PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasMember ?member ;
        :seatIncumbencyHasHouseSeat ?houseSeat .
    ?member
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party .
    ?party
        a :Party ;
        :partyName ?partyName .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?house
        a :House ;
        :houseName ?houseName .
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyGroupName .
}
WHERE {
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasMember ?member ;
        :seatIncumbencyHasHouseSeat ?houseSeat .
    ?member :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership :partyMembershipHasParty ?party .
    ?party :partyName ?partyName .
    ?houseSeat
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?constituencyGroup :constituencyGroupName ?constituencyGroupName .
    ?house :houseName ?houseName .
    OPTIONAL { ?member :personGivenName ?givenName . }
    OPTIONAL { ?member :personFamilyName ?familyName . }
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
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName .
}
WHERE {
    ?person a :Person .
    OPTIONAL { ?person :personGivenName ?givenName . }
    OPTIONAL { ?person :personFamilyName ?familyName . }

    FILTER (REGEX(STR(?familyName), @letters, 'i') || REGEX(STR(?givenName), @letters, 'i'))
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letters", letters);

            return BaseController.Execute(query);
        }

        // Ruby route: get '/people/a_z_letters', to: 'people#a_z_letters'
        [Route("a-z", Name = "PersonAToZ")]
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
    ?person a :Person.
    ?person :personFamilyName ?familyName .

    BIND(ucase(SUBSTR(?familyName, 1, 1)) as ?firstLetter)
    }
}";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.Execute(query);
        }
    }
}