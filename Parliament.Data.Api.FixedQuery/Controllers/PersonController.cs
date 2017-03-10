namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("people")]
    public class PersonController : BaseController
    {
        // Ruby route: resources :people, only: [:index]
        [Route("", Name = "PersonIndex")]
        [HttpGet]
        public Graph Index()
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
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
}
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.Execute(query);
        }
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
        :memberHasIncumbency ?incumbency .
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
       	:incumbencyEndDate ?incumbencyEndDate ;
        :incumbencyStartDate ?incumbencyStartDate ;
		:seatIncumbencyHasHouseSeat ?seat ;
        :incumbencyHasContactPoint ?contactPoint .
    ?houseIncumbency 
        a :HouseIncumbency ;
        :incumbencyEndDate ?incumbencyEndDate ;
        :incumbencyStartDate ?incumbencyStartDate ;
        :houseIncumbencyHasHouse ?house ;
        :incumbencyHasContactPoint ?contactPoint .
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
    ?house 
        a :House ;
       	:houseName ?houseName .
    }
WHERE {
    BIND(@id AS ?person)
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personOtherNames ?otherName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL {
        ?person :memberHasIncumbency ?incumbency .
        OPTIONAL { ?incumbency :incumbencyEndDate ?incumbencyEndDate . }
        ?incumbency :incumbencyStartDate ?incumbencyStartDate .
        {
            ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency )
            ?houseIncumbency :houseIncumbencyHasHouse ?house .
            ?house :houseName ?houseName .
        } 
        UNION {
            ?incumbency a :SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency )
            ?seatIncumbency :seatIncumbencyHasHouseSeat ?seat .
            ?seat :houseSeatHasConstituencyGroup ?constituency .
            ?seat :houseSeatHasHouse ?house .
            ?house :houseName ?houseName .
            ?constituency :constituencyGroupName ?constituencyName .
        }
    OPTIONAL {
        ?incumbency :incumbencyHasContactPoint ?contactPoint .
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
    } 
OPTIONAL {
    ?person :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership :partyMembershipHasParty ?party .
    ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .
    OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
    ?party :partyName ?partyName .
    }
}
";

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
}
";

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
}
";

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
   	?person :memberHasIncumbency ?incumbency .
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
    ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .
    OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
    ?party :partyName ?partyName .
}
";

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
}
";

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
}
";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.Execute(query);
        }

        // Ruby route: resources :people, only: [:index] do get '/constituencies', to: 'people#constituencies' end
        [Route("{id:guid}/constituencies", Name = "PersonConstituencies")]
        [HttpGet]
        public Graph Constituencies(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?person 
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName .
 	?constituency
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName ;
        :constituencyGroupStartDate ?constituencyStartDate ;
        :constituencyGroupEndDate ?constituencyEndDate ;
        :constituencyGroupHasHouseSeat ?seat .
    ?seat
      	a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyEndDate ?seatIncumbencyEndDate ;
        :incumbencyStartDate ?seatIncumbencyStartDate .
    }
WHERE {
    BIND(@personid AS ?person)
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL {
  	    ?person :memberHasIncumbency ?seatIncumbency .
       	?seatIncumbency a :SeatIncumbency .
   	    ?seatIncumbency :seatIncumbencyHasHouseSeat ?seat .
   	    ?seat :houseSeatHasConstituencyGroup ?constituency .
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate .
        ?constituency :constituencyGroupName ?constituencyName .
        ?constituency :constituencyGroupStartDate ?constituencyStartDate .
        OPTIONAL { ?constituency :constituencyGroupEndDate ?constituencyEndDate . }
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            query.SetUri("personid", new Uri(BaseController.instance, id));
            return BaseController.Execute(query);
        }

        // Ruby route: resources :people, only: [:index] doget '/constituencies/current', to: 'people#current_constituency' end
        [Route("{id:guid}/constituencies/current", Name = "PersonCurrentConstituency")]
        [HttpGet]
        public Graph CurrentConstituency(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName .
   	?constituency
     	a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName ;
        :constituencyGroupStartDate ?constituencyStartDate ;
        :constituencyGroupHasHouseSeat ?seat .
    ?seat
     	a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyStartDate ?seatIncumbencyStartDate .
    }
WHERE {
    BIND(@personid AS ?person)
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL {
  	    ?person :memberHasIncumbency ?seatIncumbency .
       	?seatIncumbency a :SeatIncumbency .
  	    FILTER NOT EXISTS { ?seatIncumbency a :PastIncumbency . }
   	    ?seatIncumbency :seatIncumbencyHasHouseSeat ?seat .
   	    ?seat :houseSeatHasConstituencyGroup ?constituency .
        ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate .
        ?constituency :constituencyGroupName ?constituencyName .
        ?constituency :constituencyGroupStartDate ?constituencyStartDate .
    }
}
";
            var query = new SparqlParameterizedString(queryString);
            query.SetUri("personid", new Uri(BaseController.instance, id));
        
            return BaseController.Execute(query);
        }

        // Ruby route: resources :people, only: [:index] do get '/parties', to: 'people#parties' end
        [Route("{id:guid}/parties", Name = "PersonParties")]
        [HttpGet]
        public Graph Parties(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
   	?person 
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName .
    ?party
        a :Party ;
        :partyName ?partyName ;
        :partyHasPartyMembership ?partyMembership .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipStartDate ?partyMembershipStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    }
WHERE {
    BIND(@personid AS ?person)
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL {
        ?person :partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership :partyMembershipHasParty ?party .
        ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
        ?party :partyName ?partyName .
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, id));

            return BaseController.Execute(query);
        }

        // Ruby route: resources :people, only: [:index] do get '/parties/current', to: 'people#current_party' end
        [Route("{id:guid}/parties/current", Name = "PersonCurrentParty")]
        [HttpGet]
        public Graph CurrentParty(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
  	?person 
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName .
    ?party
     	a :Party ;
        :partyName ?partyName ;
        :partyHasPartyMembership ?partyMembership .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipStartDate ?partyMembershipStartDate .
    }
WHERE {
    BIND(@personid AS ?person)
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL {
        ?person :partyMemberHasPartyMembership ?partyMembership .
        FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
        ?partyMembership :partyMembershipHasParty ?party .
        ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .
        ?party :partyName ?partyName .
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, id));

            return BaseController.Execute(query);
        }

        // Ruby route: resources :people, only: [:index] do get '/contact_points',to: 'people#contact_points' end
        // note: query currently only really returns parliamentary contact point, not "contact points"
       
        [Route("{id:guid}/contact_points", Name = "PersonContactPoints")]
        [HttpGet]
        public Graph ContactPoints(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasIncumbency ?incumbency .
 	?incumbency
      	a :Incumbency ;
      	:incumbencyHasContactPoint ?contactPoint .
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
    }
WHERE {
    BIND(@personid AS ?person)
 	OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL {
      	?person :memberHasIncumbency ?incumbency .
        FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
	    ?incumbency :incumbencyHasContactPoint ?contactPoint .
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
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, id));

            return BaseController.Execute(query);
        }

        // Ruby route: resources :people, only: [:index] do get '/houses',to: 'people#houses' end
        [Route("{id:guid}/houses", Name = "PersonHouses")]
        [HttpGet]
        public Graph Houses(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName .
 	?house
        a :House ;
        :houseName ?houseName ;
        :houseHasHouseSeat ?houseSeat ;
        :houseHasHouseIncumbency ?houseIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyEndDate ?incumbencyEndDate ;
        :incumbencyStartDate ?incumbencyStartDate .
    ?houseSeat
      	a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?houseIncumbency
        a :HouseIncumbency ;
        :incumbencyEndDate ?incumbencyEndDate ;
        :incumbencyStartDate ?incumbencyStartDate ;
        :houseIncumbencyHasHouse ?house .
    }
WHERE {
    BIND(@personid AS ?person)
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL {
        ?person :memberHasIncumbency ?incumbency .
        OPTIONAL { ?incumbency :incumbencyEndDate ?incumbencyEndDate . }
        ?incumbency :incumbencyStartDate ?incumbencyStartDate .
        OPTIONAL {
       	    ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency )
            ?houseIncumbency :houseIncumbencyHasHouse ?house .
            ?house :houseName ?houseName .
        }
        OPTIONAL {
            ?incumbency a :SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency )
            ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
          	?houseSeat :houseSeatHasConstituencyGroup ?constituency .
    	   	?houseSeat :houseSeatHasHouse ?house .
           	?house :houseName ?houseName .
           	?constituency :constituencyGroupName ?constituencyName .
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, id));

            return BaseController.Execute(query);
        }

        // Ruby route: resources :people, only: [:index] do get '/houses/current', to: 'people#current_house' end
        [Route("{id:guid}/houses/current", Name = "PersonCurrentHouse")]
        [HttpGet]
        public Graph CurrentHouse(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName .
 	?house
        a :House ;
        :houseName ?houseName ;
        :houseHasHouseSeat ?houseSeat ;
        :houseHasHouseIncumbency ?houseIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
    	:incumbencyStartDate ?incumbencyStartDate .
    ?houseSeat
     	a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?houseIncumbency
     	a :HouseIncumbency ;
        :incumbencyStartDate ?incumbencyStartDate .
    }      
WHERE {
    BIND(@personid AS ?person)
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL {
  	    ?person :memberHasIncumbency ?incumbency .
        FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
    	?incumbency :incumbencyStartDate ?incumbencyStartDate .
        OPTIONAL {
       	    ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency )
            ?houseIncumbency :houseIncumbencyHasHouse ?house .
            ?house :houseName ?houseName .
        }
    OPTIONAL {
        ?incumbency a :SeatIncumbency .
        BIND(?incumbency AS ?seatIncumbency )
        ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasConstituencyGroup ?constituency .
    	?houseSeat :houseSeatHasHouse ?house .
        ?house :houseName ?houseName .
        ?constituency :constituencyGroupName ?constituencyName .
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, id));

            return BaseController.Execute(query);
        }
    }
}
