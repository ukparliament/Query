namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Net.Http;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("people")]
    public class PersonController : BaseController
    {
        // Ruby route: resources :people, only: [:index]
        [Route("", Name = "PersonIndex")]
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
        	a parl:Person ;
         	parl:personGivenName ?givenName ;
         	parl:personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
          <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
      }
      WHERE {
        ?person a parl:Person .
        OPTIONAL { ?person parl:personGivenName ?givenName } .
        OPTIONAL { ?person parl:personFamilyName ?familyName } .
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
      }
";

            var query = new SparqlParameterizedString(queryString);

            return Execute(query);
        }
        // Ruby route: match '/people/:person', to: 'people#show', person: /\w{8}-\w{4}-\w{4}-\w{4}-\w{12}/, via: [:get]
        [Route(@"{id:regex(^\w{8}$)}", Name = "PersonById")]
        [HttpGet]
        public HttpResponseMessage ById(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
     CONSTRUCT {
        ?person a parl:Person ;
              parl:personDateOfBirth ?dateOfBirth ;
              parl:personGivenName ?givenName ;
              parl:personOtherNames ?otherName ;
              parl:personFamilyName ?familyName ;
        	    <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        	    <http://example.com/D79B0BAC513C4A9A87C9D5AFF1FC632F> ?fullTitle ;
              parl:partyMemberHasPartyMembership ?partyMembership ;
              parl:memberHasIncumbency ?incumbency .
     	  ?contactPoint a parl:ContactPoint ;
        	  parl:email ?email ;
        	  parl:phoneNumber ?phoneNumber ;
    		    parl:contactPointHasPostalAddress ?postalAddress .
    	  ?postalAddress a parl:PostalAddress ;
            parl:addressLine1 ?addressLine1 ;
            parl:addressLine2 ?addressLine2 ;
            parl:addressLine3 ?addressLine3 ;
            parl:addressLine4 ?addressLine4 ;
            parl:addressLine5 ?addressLine5 ;
            parl:faxNumber ?faxNumber ;
        	  parl:postCode ?postCode .
    	  ?constituency a parl:ConstituencyGroup ;
             parl:constituencyGroupName ?constituencyName .
    	  ?seatIncumbency a parl:SeatIncumbency ;
        	  	parl:incumbencyEndDate ?seatIncumbencyEndDate ;
        	  	parl:incumbencyStartDate ?seatIncumbencyStartDate ;
				      parl:seatIncumbencyHasHouseSeat ?seat ;
        		  parl:incumbencyHasContactPoint ?contactPoint .
    	  ?houseIncumbency a parl:HouseIncumbency ;
        		parl:incumbencyEndDate ?houseIncumbencyEndDate ;
        	  parl:incumbencyStartDate ?houseIncumbencyStartDate ;
        		parl:houseIncumbencyHasHouse ?house1 ;
        		parl:incumbencyHasContactPoint ?contactPoint .
        ?seat a parl:HouseSeat ;
            	parl:houseSeatHasConstituencyGroup ?constituency ;
            	parl:houseSeatHasHouse ?house2 .
    		?party a parl:Party ;
             	parl:partyName ?partyName .
    		?partyMembership a parl:PartyMembership ;
        	  	parl:partyMembershipStartDate ?partyMembershipStartDate ;
        	  	parl:partyMembershipEndDate ?partyMembershipEndDate ;
				      parl:partyMembershipHasParty ?party .
    		?house1 a parl:House ;
            	parl:houseName ?houseName1 .
        ?house2 a parl:House ;
            	parl:houseName ?houseName2 .
      }
      WHERE {
        BIND(@id AS ?person)

        ?person a parl:Person .
        OPTIONAL { ?person parl:personGivenName ?givenName } .
        OPTIONAL { ?person parl:personOtherNames ?otherName } .
        OPTIONAL { ?person parl:personFamilyName ?familyName } .
    	  OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        OPTIONAL { ?person <http://example.com/D79B0BAC513C4A9A87C9D5AFF1FC632F> ?fullTitle } .
    	  OPTIONAL {
    	      ?person parl:memberHasIncumbency ?incumbency .
            OPTIONAL
        	    {
        	      ?incumbency a parl:HouseIncumbency .
                BIND(?incumbency AS ?houseIncumbency)
                ?houseIncumbency parl:houseIncumbencyHasHouse ?house1 .
            	  ?house1 parl:houseName ?houseName1 .
                ?houseIncumbency parl:incumbencyStartDate ?houseIncumbencyStartDate .
                OPTIONAL { ?houseIncumbency parl:incumbencyEndDate ?houseIncumbencyEndDate . }
        	    }
              OPTIONAL {
        	      ?incumbency a parl:SeatIncumbency .
                BIND(?incumbency AS ?seatIncumbency)
                ?seatIncumbency parl:seatIncumbencyHasHouseSeat ?seat .
            	  ?seat parl:houseSeatHasConstituencyGroup ?constituency .
    	      	  ?seat parl:houseSeatHasHouse ?house2 .
            	  ?house2 parl:houseName ?houseName2 .
            	  ?constituency parl:constituencyGroupName ?constituencyName .
                ?seatIncumbency parl:incumbencyStartDate ?seatIncumbencyStartDate .
                OPTIONAL { ?seatIncumbency parl:incumbencyEndDate ?seatIncumbencyEndDate . }
        	    }
              OPTIONAL {
        	        ?incumbency parl:incumbencyHasContactPoint ?contactPoint .
        	        OPTIONAL { ?contactPoint parl:phoneNumber ?phoneNumber . }
        	        OPTIONAL { ?contactPoint parl:email ?email . }
        	        OPTIONAL {
        	          ?contactPoint parl:contactPointHasPostalAddress ?postalAddress .
				            OPTIONAL { ?postalAddress parl:addressLine1 ?addressLine1 . }
				            OPTIONAL { ?postalAddress parl:addressLine2 ?addressLine2 . }
        	    	    OPTIONAL { ?postalAddress parl:addressLine3 ?addressLine3 . }
        	    	    OPTIONAL { ?postalAddress parl:addressLine4 ?addressLine4 . }
        	    	    OPTIONAL { ?postalAddress parl:addressLine5 ?addressLine5 . }
        	    	    OPTIONAL { ?postalAddress parl:faxNumber ?faxNumber . }
        	    	    OPTIONAL { ?postalAddress parl:postCode ?postCode . }
        	        }
    	          }
            }
            OPTIONAL {
    	        ?person parl:partyMemberHasPartyMembership ?partyMembership .
              ?partyMembership parl:partyMembershipHasParty ?party .
              ?partyMembership parl:partyMembershipStartDate ?partyMembershipStartDate .
              OPTIONAL { ?partyMembership parl:partyMembershipEndDate ?partyMembershipEndDate . }
              ?party parl:partyName ?partyName .
            }
          }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(instance, id));
            
            return Execute(query);
        }

        // Ruby route: get '/people/:letter', to: 'people#letters', letter: /[A-Za-z]/, via: [:get]
        // TODO: accents ({x:regex} with unicode alpha)?
        // TODO: REGEX ignore case?
        [Route("{initial:maxlength(1)}", Name = "PersonByInitial")]
        [HttpGet]
        public HttpResponseMessage ByInitial(string initial)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
        	a parl:Person ;
         	parl:personGivenName ?givenName ;
         	parl:personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
          <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
      }
      WHERE {
        ?person a parl:Person .
        OPTIONAL { ?person parl:personGivenName ?givenName } .
        OPTIONAL { ?person parl:personFamilyName ?familyName } .
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .


        FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return Execute(query);

        }

        // Ruby route: get '/people/lookup', to: 'people#lookup'
        // TODO: validate source against actual properties?
        // TODO: validate cource and id combnation?
        // TODO: source could have numbers?
        [Route(@"lookup/{source:regex(^\p{L}+$)}/{id}", Name = "PersonLookup")]
        [HttpGet]
        public HttpResponseMessage ByExternalIdentifier(string source, string id)
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

            return Execute(query);
        }

        // Ruby: get '/people/members', to: 'members#index'
        [Route("members", Name = "MemberIndex")]
        [HttpGet]
        public HttpResponseMessage Member()
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
         ?houseSeat
          a parl:HouseSeat ;
          parl:houseSeatHasConstituencyGroup ?constituencyGroup .
        ?person
          a parl:Person ;
          parl:personGivenName ?givenName ;
          parl:personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
          <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
          parl:memberHasIncumbency ?incumbency ;
          parl:partyMemberHasPartyMembership ?partyMembership .
        ?seatIncumbency
          a parl:SeatIncumbency ;
          parl:seatIncumbencyHasHouseSeat ?houseSeat ;
          parl:incumbencyEndDate ?seatIncumbencyEndDate .
    	  ?houseIncumbency
          a parl:HouseIncumbency ;
          parl:incumbencyEndDate ?houseIncumbencyEndDate .
        ?constituencyGroup
          a parl:ConstituencyGroup ;
          parl:constituencyGroupName ?constituencyName .
        ?partyMembership
          a parl:PartyMembership ;
          parl:partyMembershipHasParty ?party .
        ?party
          a parl:Party ;
          parl:partyName ?partyName .
      }
      WHERE {
    	  ?person a parl:Person ;
                parl:memberHasIncumbency ?incumbency .
        OPTIONAL { ?person parl:personGivenName ?givenName . }
        OPTIONAL { ?person parl:personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

         { ?incumbency a parl:HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency)
            OPTIONAL { ?houseIncumbency parl:incumbencyEndDate ?houseIncumbencyEndDate . }
   		    }
         UNION {
          ?incumbency a parl:SeatIncumbency .
          BIND(?incumbency AS ?seatIncumbency)
          ?seatIncumbency parl:seatIncumbencyHasHouseSeat ?houseSeat .
          OPTIONAL { ?seatIncumbency parl:incumbencyEndDate ?seatIncumbencyEndDate . }
    		  OPTIONAL { ?houseSeat parl:houseSeatHasConstituencyGroup ?constituencyGroup .
       		?constituencyGroup parl:constituencyGroupName ?constituencyName . }
   		  }

        OPTIONAL {
    	    ?person parl:partyMemberHasPartyMembership ?partyMembership .
          FILTER NOT EXISTS { ?partyMembership a parl:PastPartyMembership . }
          ?partyMembership parl:partyMembershipHasParty ?party .
          ?party parl:partyName ?partyName .
        }
      }
";

            return Execute(queryString);
        }

        // Ruby route: get '/people/:letters', to: 'people#lookup_by_letters'
        // TODO: letters length?
        // TODO: STR required because OPTIONAL?
        // TODO: accents?
        // TODO: could be CONTAINS?
        // TODO: letters go in STR?
        [Route(@"{letters:regex(^\p{L}+$):minlength(2)}", Name = "PersonByLetters", Order = 999)]
        [HttpGet]
        public HttpResponseMessage ByLetters(string letters)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
        	a parl:Person ;
         	parl:personGivenName ?givenName ;
         	parl:personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs
      }
      WHERE {
        ?person a parl:Person .
        OPTIONAL { ?person parl:personGivenName ?givenName } .
        OPTIONAL { ?person parl:personFamilyName ?familyName } .
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .

      

    FILTER (REGEX(STR(?displayAs), @letters, 'i'))
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letters", letters);
            return Execute(query);            
        }

        // Ruby route: get '/people/a_z_letters', to: 'people#a_z_letters'
        [Route("a_z_letters", Name = "PersonAToZ")]
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
	        ?s a parl:Person .
          ?s <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
";

            var query = new SparqlParameterizedString(queryString);
            return Execute(query);
        }

        // Ruby route: resources :people, only: [:index] do get '/constituencies', to: 'people#constituencies' end
        [Route(@"{id:regex(^\w{8}$)}/constituencies", Name = "PersonConstituencies")]
        [HttpGet]
        public HttpResponseMessage Constituencies(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person a parl:Person ;
              parl:personGivenName ?givenName ;
              parl:personFamilyName ?familyName ;
              <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
    	 ?constituency
        	  a parl:ConstituencyGroup ;
            parl:constituencyGroupName ?constituencyName ;
        	  parl:constituencyGroupStartDate ?constituencyStartDate ;
        	  parl:constituencyGroupEndDate ?constituencyEndDate .
    	  ?seat
        	  a parl:HouseSeat ;
        	  parl:houseSeatHasConstituencyGroup ?constituency .
    	  ?seatIncumbency
            a parl:SeatIncumbency ;
        	  parl:incumbencyEndDate ?seatIncumbencyEndDate ;
        	  parl:incumbencyStartDate ?seatIncumbencyStartDate ;
            parl:seatIncumbencyHasHouseSeat ?seat .
      }
      WHERE {
        BIND(@personid AS ?person)

        ?person a parl:Person .
        OPTIONAL { ?person parl:personGivenName ?givenName } .
        OPTIONAL { ?person parl:personFamilyName ?familyName } .
    	  OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .

        OPTIONAL {
    	    ?person parl:memberHasIncumbency ?seatIncumbency .
        	?seatIncumbency a parl:SeatIncumbency .
    	    ?seatIncumbency parl:seatIncumbencyHasHouseSeat ?seat .
    	    ?seat parl:houseSeatHasConstituencyGroup ?constituency .
          OPTIONAL { ?seatIncumbency parl:incumbencyEndDate ?seatIncumbencyEndDate . }
          ?seatIncumbency parl:incumbencyStartDate ?seatIncumbencyStartDate .
          ?constituency parl:constituencyGroupName ?constituencyName .
          ?constituency parl:constituencyGroupStartDate ?constituencyStartDate .
		      OPTIONAL { ?constituency parl:constituencyGroupEndDate ?constituencyEndDate . }
        }
      }
";

            var query = new SparqlParameterizedString(queryString);
            query.SetUri("personid", new Uri(BaseController.instance, id));
            return Execute(query);
        }

        // Ruby route: resources :people, only: [:index] doget '/constituencies/current', to: 'people#current_constituency' end
        [Route(@"{id:regex(^\w{8}$)}/constituencies/current", Name = "PersonCurrentConstituency")]
        [HttpGet]
        public HttpResponseMessage CurrentConstituency(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
              a parl:Person ;
              parl:personGivenName ?givenName ;
              parl:personFamilyName ?familyName ;
              <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
    	  ?constituency
        	  a parl:ConstituencyGroup ;
            parl:constituencyGroupName ?constituencyName ;
        	  parl:constituencyGroupStartDate ?constituencyStartDate ;
            parl:constituencyGroupHasHouseSeat ?seat .
    	  ?seat
        	  a parl:HouseSeat ;
        	  parl:houseSeatHasSeatIncumbency ?seatIncumbency .
    	  ?seatIncumbency
            a parl:SeatIncumbency ;
        	  parl:incumbencyStartDate ?seatIncumbencyStartDate .
      }
      WHERE {
        BIND(@personid AS ?person)

        ?person a parl:Person .
        OPTIONAL { ?person parl:personGivenName ?givenName } .
        OPTIONAL { ?person parl:personFamilyName ?familyName } .
    	  OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .

        OPTIONAL {
    	    ?person parl:memberHasIncumbency ?seatIncumbency .
        	?seatIncumbency a parl:SeatIncumbency .
    	    FILTER NOT EXISTS { ?seatIncumbency a parl:PastIncumbency . }
    	    ?seatIncumbency parl:seatIncumbencyHasHouseSeat ?seat .
    	    ?seat parl:houseSeatHasConstituencyGroup ?constituency .
          ?seatIncumbency parl:incumbencyStartDate ?seatIncumbencyStartDate .
          ?constituency parl:constituencyGroupName ?constituencyName .
          ?constituency parl:constituencyGroupStartDate ?constituencyStartDate .
        }
      }
";
            var query = new SparqlParameterizedString(queryString);
            query.SetUri("personid", new Uri(BaseController.instance, id));
        
            return Execute(query);
        }

        // Ruby route: resources :people, only: [:index] do get '/parties', to: 'people#parties' end
        [Route(@"{id:regex(^\w{8}$)}/parties", Name = "PersonParties")]
        [HttpGet]
        public HttpResponseMessage Parties(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
    	?person a parl:Person ;
              parl:personGivenName ?givenName ;
              parl:personFamilyName ?familyName ;
              <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
      ?party
        	  a parl:Party ;
            parl:partyName ?partyName .
    	?partyMembership
            a parl:PartyMembership ;
        	  parl:partyMembershipStartDate ?partyMembershipStartDate ;
        	  parl:partyMembershipEndDate ?partyMembershipEndDate ;
            parl:partyMembershipHasParty ?party .
       }
       WHERE {
          BIND(@personid AS ?person)

          ?person a parl:Person .
          OPTIONAL { ?person parl:personGivenName ?givenName } .
          OPTIONAL { ?person parl:personFamilyName ?familyName } .
    	    OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .

          OPTIONAL {
            ?person parl:partyMemberHasPartyMembership ?partyMembership .
            ?partyMembership parl:partyMembershipHasParty ?party .
            ?partyMembership parl:partyMembershipStartDate ?partyMembershipStartDate .
            OPTIONAL { ?partyMembership parl:partyMembershipEndDate ?partyMembershipEndDate . }
            ?party parl:partyName ?partyName .
          }
       }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :people, only: [:index] do get '/parties/current', to: 'people#current_party' end
        [Route(@"{id:regex(^\w{8}$)}/parties/current", Name = "PersonCurrentParty")]
        [HttpGet]
        public HttpResponseMessage CurrentParty(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
    	?person a parl:Person ;
              parl:personGivenName ?givenName ;
              parl:personFamilyName ?familyName ;
              <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
      ?party
        	  a parl:Party ;
            parl:partyName ?partyName ;
            parl:partyHasPartyMembership ?partyMembership .
    	?partyMembership
            a parl:PartyMembership ;
        	  parl:partyMembershipStartDate ?partyMembershipStartDate .
       }
       WHERE {
          BIND(@personid AS ?person)

          ?person a parl:Person .
          OPTIONAL { ?person parl:personGivenName ?givenName } .
          OPTIONAL { ?person parl:personFamilyName ?familyName } .
    	    OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .

          OPTIONAL {
            ?person parl:partyMemberHasPartyMembership ?partyMembership .
    	      FILTER NOT EXISTS { ?partyMembership a parl:PastPartyMembership . }
            ?partyMembership parl:partyMembershipHasParty ?party .
            ?partyMembership parl:partyMembershipStartDate ?partyMembershipStartDate .
            ?party parl:partyName ?partyName .
    	    }
       }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :people, only: [:index] do get '/contact_points',to: 'people#contact_points' end
        // note: query currently only really returns parliamentary contact point, not "contact points"
       
        [Route(@"{id:regex(^\w{8}$)}/contact_points", Name = "PersonContactPoints")]
        [HttpGet]
        public HttpResponseMessage ContactPoints(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
          a parl:Person ;
          parl:personGivenName ?givenName ;
          parl:personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
          parl:memberHasIncumbency ?incumbency .
    	  ?incumbency
        	a parl:Incumbency ;
        	parl:incumbencyHasContactPoint ?contactPoint .
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
      }
      WHERE {
        BIND(@personid AS ?person)

        ?person a parl:Person .
    	  OPTIONAL { ?person parl:personGivenName ?givenName } .
        OPTIONAL { ?person parl:personFamilyName ?familyName } .
    	  OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .

        OPTIONAL {
        	?person parl:memberHasIncumbency ?incumbency .
          FILTER NOT EXISTS { ?incumbency a parl:PastIncumbency . }
	        ?incumbency parl:incumbencyHasContactPoint ?contactPoint .
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
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :people, only: [:index] do get '/houses',to: 'people#houses' end
        [Route(@"{id:regex(^\w{8}$)}/houses", Name = "PersonHouses")]
        [HttpGet]
        public HttpResponseMessage Houses(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
            a parl:Person ;
            parl:personGivenName ?givenName ;
            parl:personFamilyName ?familyName ;
            <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
    	  ?house1
            a parl:House ;
    			  parl:houseName ?houseName1 .
        ?house2
            a parl:House ;
    			  parl:houseName ?houseName2 .
    	  ?seatIncumbency
            a parl:Incumbency ;
        	  parl:incumbencyEndDate ?incumbencyEndDate ;
        	  parl:incumbencyStartDate ?incumbencyStartDate ;
            parl:seatIncumbencyHasHouseSeat ?houseSeat .
    		?houseSeat
        		a parl:HouseSeat ;
        		parl:houseSeatHasHouse ?house1 .
    		?houseIncumbency
        		a parl:Incumbency ;
        		parl:incumbencyEndDate ?incumbencyEndDate ;
        	  parl:incumbencyStartDate ?incumbencyStartDate ;
        		parl:houseIncumbencyHasHouse ?house2 .
      }
      WHERE {
        BIND(@personid AS ?person)

        ?person a parl:Person .
        OPTIONAL { ?person parl:personGivenName ?givenName } .
        OPTIONAL { ?person parl:personFamilyName ?familyName } .
    	  OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .

        OPTIONAL {
    	     ?person parl:memberHasIncumbency ?incumbency .
           OPTIONAL { ?incumbency parl:incumbencyEndDate ?incumbencyEndDate . }
    	     ?incumbency parl:incumbencyStartDate ?incumbencyStartDate .

           OPTIONAL {
        	   ?incumbency a parl:HouseIncumbency .
             BIND(?incumbency AS ?houseIncumbency )
             ?houseIncumbency parl:houseIncumbencyHasHouse ?house2 .
             ?house2 parl:houseName ?houseName2 .
        	 }
            OPTIONAL {
        	    ?incumbency a parl:SeatIncumbency .
              BIND(?incumbency AS ?seatIncumbency )
              ?seatIncumbency parl:seatIncumbencyHasHouseSeat ?houseSeat .
            	?houseSeat parl:houseSeatHasConstituencyGroup ?constituency .
    	      	?houseSeat parl:houseSeatHasHouse ?house1 .
            	?house1 parl:houseName ?houseName1 .
            	?constituency parl:constituencyGroupName ?constituencyName .
        	  }
        }
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :people, only: [:index] do get '/houses/current', to: 'people#current_house' end
        [Route(@"{id:regex(^\w{8}$)}/houses/current", Name = "PersonCurrentHouse")]
        [HttpGet]
        public HttpResponseMessage CurrentHouse(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
            a parl:Person ;
            parl:personGivenName ?givenName ;
            parl:personFamilyName ?familyName ;
            <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
    	  ?house
            a parl:House ;
    			  parl:houseName ?houseName ;
            parl:houseHasHouseSeat ?houseSeat ;
            parl:houseHasHouseIncumbency ?houseIncumbency .
    	  ?seatIncumbency
            a parl:SeatIncumbency ;
        	  parl:incumbencyStartDate ?incumbencyStartDate .
    		?houseSeat
        		a parl:HouseSeat ;
        		parl:houseSeatHasSeatIncumbency ?seatIncumbency .
    		?houseIncumbency
        		a parl:HouseIncumbency ;
        	  parl:incumbencyStartDate ?incumbencyStartDate .
      }
      WHERE {
        BIND(@personid AS ?person)

        ?person a parl:Person .
        OPTIONAL { ?person parl:personGivenName ?givenName } .
        OPTIONAL { ?person parl:personFamilyName ?familyName } .
    	  OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .

        OPTIONAL {
    	     ?person parl:memberHasIncumbency ?incumbency .
           FILTER NOT EXISTS { ?incumbency a parl:PastIncumbency . }
    	     ?incumbency parl:incumbencyStartDate ?incumbencyStartDate .

           OPTIONAL {
        	   ?incumbency a parl:HouseIncumbency .
             BIND(?incumbency AS ?houseIncumbency )
             ?houseIncumbency parl:houseIncumbencyHasHouse ?house .
             ?house parl:houseName ?houseName .
        	 }
            OPTIONAL {
        	    ?incumbency a parl:SeatIncumbency .
              BIND(?incumbency AS ?seatIncumbency )
              ?seatIncumbency parl:seatIncumbencyHasHouseSeat ?houseSeat .
            	?houseSeat parl:houseSeatHasConstituencyGroup ?constituency .
    	      	?houseSeat parl:houseSeatHasHouse ?house .
            	?house parl:houseName ?houseName .
            	?constituency parl:constituencyGroupName ?constituencyName .
        	  }
        }
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, id));

            return Execute(query);
        }
    }
}
