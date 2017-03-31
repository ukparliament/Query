namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Net.Http;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("houses")]
    public class HouseController : BaseController
    {
        // Ruby route: match '/houses/:house', to: 'houses#show', house: /\w{8}-\w{4}-\w{4}-\w{4}-\w{12}/, via: [:get]

        [Route(@"{id:regex(^\w{8}$)}", Name = "HouseById")]
        [HttpGet]
        public HttpResponseMessage ById(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
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
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, id));

            return Execute(query);

        }

        // Ruby route: get '/houses/lookup', to: 'houses#lookup'
        // NOTE: this does not work as we don't have house mnis IDs, as that feels like overkill, and overreliance on MNIS

        [Route(@"lookup/{source:regex(^\p{L}+$)}/{id}", Name = "HouseLookup")]
        [HttpGet]
        public HttpResponseMessage Lookup(string source, string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?house
           a :House .
      }
      WHERE {
        BIND(@id AS ?id)
        BIND(@source AS ?source)
        ?house a :House.
        ?house ?source ?id.
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("source", new Uri(BaseController.schema, source));
            query.SetLiteral("id", id);

            return Execute(query);
        }

        // Ruby route:   get '/houses/:letters', to: 'houses#lookup_by_letters'
        [Route(@"{letters:regex(^\p{L}+$):minlength(2)}", Name = "HouseByLetters", Order = 999)]
        [HttpGet]
        public HttpResponseMessage ByLetters(string letters)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?house
        	a :House ;
         	:houseName ?houseName .
      }
      WHERE {
        ?house a :House .
        ?house :houseName ?houseName .

        FILTER CONTAINS(LCASE(?houseName), LCASE(@letters))
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letters", letters);

            return Execute(query);
        }
        // Ruby route: resources :houses, only: [:index] 
        [Route("", Name = "HouseIndex")]
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
          ?house
            a :House ;
        	  :houseName ?houseName .
      }
      WHERE {
          ?house
             a :House ;
    			   :houseName ?houseName .
      }
";

            var query = new SparqlParameterizedString(queryString);

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get '/members', to: 'houses#members' end
        [Route(@"{id:regex(^\w{8}$)}/members", Name = "HouseMembers")]
        [HttpGet]
        public HttpResponseMessage Members(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
        	  a :Person ;
            :personGivenName ?givenName ;
            :personFamilyName ?familyName ;
            <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
            <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
            :memberHasIncumbency ?incumbency .
    	  ?house
        	  a :House ;
        	  :houseName ?houseName .
    	  ?incumbency
            a :Incumbency ;
        	  :incumbencyEndDate ?incumbencyEndDate ;
        	  :incumbencyStartDate ?incumbencyStartDate .
      }
      WHERE {
        BIND(@houseid AS ?house)

        ?house a :House ;
               :houseName ?houseName .
    	  ?person a :Member .
    	  ?incumbency :incumbencyHasMember ?person ;
       				      :incumbencyStartDate ?incumbencyStartDate .

        OPTIONAL { ?incumbency :incumbencyEndDate ?incumbencyEndDate . }
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

    	  {
    	      ?incumbency :houseIncumbencyHasHouse ?house .
    	  }
    	  UNION {
          	?incumbency :seatIncumbencyHasHouseSeat ?seat .
          	?seat :houseSeatHasHouse ?house .
    	  }
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get '/members/current', to: 'houses#current_members' end
        [Route(@"{id:regex(^\w{8}$)}/members/current", Name = "HouseCurrentMembers")]
        [HttpGet]
        public HttpResponseMessage CurrentMembers(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
        	  a :Person ;
            :personGivenName ?givenName ;
            :personFamilyName ?familyName ;
            <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
            <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        	  :partyMemberHasPartyMembership ?partyMembership ;
            :memberHasIncumbency ?incumbency .
    	  ?house
        	  a :House ;
        	  :houseName ?houseName .
    	  ?seatIncumbency
            a :SeatIncumbency ;
        	  :incumbencyStartDate ?incumbencyStartDate ;
            :seatIncumbencyHasHouseSeat ?seat .
    	  ?houseIncumbency
            a :HouseIncumbency ;
        	  :incumbencyStartDate ?incumbencyStartDate ;
            :houseIncumbencyHasHouse ?house .
        ?seat
            a :HouseSeat ;
            :houseSeatHasConstituencyGroup ?constituency .
    	  ?partyMembership
        	  a :PartyMembership ;
        	  :partyMembershipHasParty ?party .
    	  ?party
        	  a :Party ;
        	  :partyName ?partyName .
        ?constituency
        	a :ConstituencyGroup ;
        	:constituencyGroupName ?constituencyName .
      }
      WHERE {
        BIND(@houseid AS ?house)

        ?house a :House ;
               :houseName ?houseName .
    	  ?person a :Member .
    	  ?person :partyMemberHasPartyMembership ?partyMembership .
    	  FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
    	  ?partyMembership :partyMembershipHasParty ?party .
    	  ?party :partyName ?partyName .
    	  ?incumbency :incumbencyHasMember ?person ;
       			       :incumbencyStartDate ?incumbencyStartDate .
    	  FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }

    	  {
    	      ?incumbency :houseIncumbencyHasHouse ?house .
            BIND(?incumbency AS ?houseIncumbency)
    	  }

    	  UNION {
        	?incumbency :seatIncumbencyHasHouseSeat ?seat .
        	?seat :houseSeatHasHouse ?house .
        	?seat :houseSeatHasConstituencyGroup ?constituency .
        	?constituency :constituencyGroupName ?constituencyName .
          BIND(?incumbency AS ?seatIncumbency)
    	  }

        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get '/parties', to: 'houses#parties' end
        [Route(@"{id:regex(^\w{8}$)}/parties", Name = "HouseParties")]
        [HttpGet]
        public HttpResponseMessage Parties(string id)
        {
            var queryString = @"

PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
       ?house
        	a :House ;
        	:houseName ?houseName .
        ?party
          a :Party ;
          :partyName ?partyName .
      }
      WHERE {
        BIND(@houseid as ?house)

        ?house a :House ;
               :houseName ?houseName .
        ?person a :Member .
        ?incumbency :incumbencyHasMember ?person ;
                    :incumbencyStartDate ?incStartDate .
        OPTIONAL { ?incumbency :incumbencyEndDate ?incumbencyEndDate . }

        {
            ?incumbency :houseIncumbencyHasHouse ?house .
        }
        UNION
        {
            ?incumbency :seatIncumbencyHasHouseSeat ?houseSeat .
            ?houseSeat :houseSeatHasHouse ?house .
        }

        ?partyMembership :partyMembershipHasPartyMember ?person ;
            			:partyMembershipHasParty ?party ;
            			:partyMembershipStartDate ?pmStartDate .
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
        ?party :partyName ?partyName.

        BIND(COALESCE(?partyMembershipEndDate,now()) AS ?pmEndDate)
        BIND(COALESCE(?incumbencyEndDate,now()) AS ?incEndDate)

        FILTER (
            (?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
            (?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
        )
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get '/parties/current', to: 'houses#current_parties' end
        [Route(@"{id:regex(^\w{8}$)}/parties/current", Name = "HouseCurrentParties")]
        [HttpGet]
        public HttpResponseMessage CurrentParties(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?house
        	a :House ;
        	:houseName ?houseName .
        ?party
          a :Party ;
          :partyName ?partyName ;
    	    :count ?memberCount .
      }
       WHERE {
        SELECT ?party ?house ?houseName ?partyName (COUNT(?person) AS ?memberCount)
    	  WHERE {

          BIND(@houseid AS ?house)

          ?house a :House ;
                 :houseName ?houseName .
          ?person a :Member .
          ?incumbency :incumbencyHasMember ?person .
          FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
    		  ?person :partyMemberHasPartyMembership ?partyMembership .
          FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
    		  ?partyMembership :partyMembershipHasParty ?party .
    		  ?party :partyName ?partyName .

    		  {
    		      ?incumbency :houseIncumbencyHasHouse ?house .
    		  }

    		  UNION {
          		?incumbency :seatIncumbencyHasHouseSeat ?seat .
          		?seat :houseSeatHasHouse ?house .
    		  }
        }
        GROUP BY ?party ?house ?houseName ?partyName
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get '/parties/:party_id', to: 'houses#party' end
        // this route doesn't seem particularly useful? should it not return the party's members?
        // DOUBLE QUERY
        [Route(@"{houseid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}", Name = "HousePartyById")]
        [HttpGet]
        public HttpResponseMessage PartyById(string houseid, string partyid)
        {
            var queryString = @"

PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?house
        a :House ;
        :houseName ?houseName .
    ?party
        a :Party ;
        :partyName ?partyName .
    ?party :count ?currentMemberCount .
}
WHERE { 
    {select * where {
            BIND(@houseid AS ?house)
            ?house :houseName ?houseName .
            OPTIONAL {
                BIND(@partyid AS ?party)
                ?person a :Member .
                ?person :partyMemberHasPartyMembership ?partyMembership .
                ?partyMembership :partyMembershipHasParty ?party .
                ?party :partyName ?partyName .
                ?incumbency :incumbencyHasMember ?person .
                {
                    ?incumbency :houseIncumbencyHasHouse ?house .
                }
                UNION 
                {
                    ?incumbency :seatIncumbencyHasHouseSeat ?seat .
                    ?seat :houseSeatHasHouse ?house .
                }
            }
        }
    }
    UNION
    {
        SELECT ?party (COUNT(?currentMember) AS ?currentMemberCount) WHERE {
            BIND(@houseid AS ?house)
            BIND(@partyid AS ?party)
            ?house a :House .
            ?party a :Party .
            OPTIONAL {
                ?party :partyHasPartyMembership ?partyMembership .
                FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
                ?partyMembership :partyMembershipHasPartyMember ?currentMember .
                ?currentMember :memberHasIncumbency ?incumbency .
                FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
                {
                    ?incumbency :houseIncumbencyHasHouse ?house .
                }
                UNION {
                    ?incumbency :seatIncumbencyHasHouseSeat ?seat .
                    ?seat :houseSeatHasHouse ?house .
                }
            }
        }
        GROUP BY ?party
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query.SetUri("partyid", new Uri(BaseController.instance, partyid));


            return Execute(query);


        }

        // Ruby route: resources :houses, only: [:index] do match '/members/:letter', to: 'houses#members_letters', letter: /[A-Za-z]/, via: [:get] end
        [Route(@"{houseid:regex(^\w{8}$)}/members/{initial:maxlength(1)}", Name = "HouseMembersByInitial")]
        [HttpGet]
        public HttpResponseMessage MembersByInitial(string houseid, string initial)
        {
            var queryString = @"

PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
        	  a :Person ;
            :personGivenName ?givenName ;
            :personFamilyName ?familyName ;
            <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
            <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
            :memberHasIncumbency ?incumbency .
    	  ?house
        	  a :House ;
        	  :houseName ?houseName .
    	  ?incumbency
            a :Incumbency ;
        	  :incumbencyEndDate ?incumbencyEndDate ;
        	  :incumbencyStartDate ?incumbencyStartDate .
      }
      WHERE {
        BIND(@houseid AS ?house)

        ?house a :House ;
               :houseName ?houseName .
    	  ?person a :Member .
    	  ?incumbency :incumbencyHasMember ?person ;
       				      :incumbencyStartDate ?incumbencyStartDate .

        OPTIONAL { ?incumbency :incumbencyEndDate ?incumbencyEndDate . }
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

    	  {
    	      ?incumbency :houseIncumbencyHasHouse ?house .
    	  }
    	  UNION {
          	?incumbency :seatIncumbencyHasHouseSeat ?seat .
          	?seat :houseSeatHasHouse ?house .
    	  }

        FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
}

";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query.SetLiteral("initial", initial);

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get '/members/a_z_letters', to: 'houses#a_z_letters_members' end
        [Route(@"{id:regex(^\w{8}$)}/members/a_z_letters", Name = "HouseMembersAToZ")]
        [HttpGet]
        public HttpResponseMessage MembersAToZLetters(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
         _:x :value ?firstLetter .
      }
      WHERE {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)

          ?house a :House ;
	               :houseName ?houseName .
    	    ?person a :Member ;
                  <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
    	    ?incumbency :incumbencyHasMember ?person .

    	    {
    	        ?incumbency :houseIncumbencyHasHouse ?house .
    	    }

    	    UNION {
            	?incumbency :seatIncumbencyHasHouseSeat ?seat .
            	?seat :houseSeatHasHouse ?house .
    	    }

          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, id));

            return Execute(query);
        }
        // Ruby route: resources :houses, only: [:index] do match '/members/current/:letter', to: 'houses#current_members_letters', letter: /[A-Za-z]/, via: [:get] end
        [Route(@"{houseid:regex(^\w{8}$)}/members/current/{initial:maxlength(1)}", Name = "HouseCurrentMembersByInitial")]
        [HttpGet]
        public HttpResponseMessage CurrentMembersByInitial(string houseid, string initial)
        {
            var queryString = @"

PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
        	  a :Person ;
            :personGivenName ?givenName ;
            :personFamilyName ?familyName ;
            <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
            <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        	  :partyMemberHasPartyMembership ?partyMembership ;
            :memberHasIncumbency ?incumbency .
    	  ?house
        	  a :House ;
        	  :houseName ?houseName .
    	  ?seatIncumbency
            a :SeatIncumbency ;
        	  :incumbencyStartDate ?incumbencyStartDate ;
            :seatIncumbencyHasHouseSeat ?seat .
    	  ?houseIncumbency
            a :HouseIncumbency ;
        	  :incumbencyStartDate ?incumbencyStartDate ;
            :houseIncumbencyHasHouse ?house .
        ?seat
            a :HouseSeat ;
            :houseSeatHasConstituencyGroup ?constituency .
    	  ?partyMembership
        	  a :PartyMembership ;
        	  :partyMembershipHasParty ?party .
    	  ?party
        	  a :Party ;
        	  :partyName ?partyName .
        ?constituency
        	a :ConstituencyGroup ;
        	:constituencyGroupName ?constituencyName .
      }
      WHERE {
        BIND(@houseid AS ?house)

        ?house a :House ;
               :houseName ?houseName .
    	  ?person a :Member .
    	  ?person :partyMemberHasPartyMembership ?partyMembership .
    	  FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
    	  ?partyMembership :partyMembershipHasParty ?party .
    	  ?party :partyName ?partyName .
    	  ?incumbency :incumbencyHasMember ?person ;
       			       :incumbencyStartDate ?incumbencyStartDate .
    	  FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }

    	  {
    	      ?incumbency :houseIncumbencyHasHouse ?house .
            BIND(?incumbency AS ?houseIncumbency)
    	  }

    	  UNION {
        	?incumbency :seatIncumbencyHasHouseSeat ?seat .
        	?seat :houseSeatHasHouse ?house .
        	?seat :houseSeatHasConstituencyGroup ?constituency .
        	?constituency :constituencyGroupName ?constituencyName .
          BIND(?incumbency AS ?seatIncumbency)
    	  }

        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

        FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query.SetLiteral("initial", initial);

            return Execute(query);
        }
        // Ruby route: resources :houses, only: [:index] do get '/members/current/a_z_letters', to: 'houses#a_z_letters_members' end
        [Route(@"{id:regex(^\w{8}$)}/members/current/a_z_letters", Name = "HouseCurrentMembersAToZ")]
        [HttpGet]
        public HttpResponseMessage CurrentMembersAToZLetters(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
         _:x :value ?firstLetter .
      }
      WHERE {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)

          ?house a :House ;
	               :houseName ?houseName .
    	    ?person a :Member;
       			      <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
    	    ?incumbency :incumbencyHasMember ?person .
    	    FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }

    	    {
    	        ?incumbency :houseIncumbencyHasHouse ?house .
    	    }

    	    UNION {
            	?incumbency :seatIncumbencyHasHouseSeat ?seat .
            	?seat :houseSeatHasHouse ?house .
    	    }

          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get '/parties/:party_id/members', to: 'houses#party_members' end
        [Route(@"{houseid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}/members", Name = "HousePartyMembers")]
        [HttpGet]
        public HttpResponseMessage PartyMembers(string houseid, string partyid)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
    	?person
        	a :Person ;
        	:personGivenName ?givenName ;
          :personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
          <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        	:partyMemberHasPartyMembership ?partyMembership ;
        	:memberHasIncumbency ?incumbency .
    	?house
        	a :House ;
        	:houseName ?houseName .
    	?party
        	a :Party ;
        	:partyName ?partyName .
    	?partyMembership
        	a :PartyMembership ;
        	:partyMembershipStartDate ?partyMembershipStartDate ;
        	:partyMembershipEndDate ?partyMembershipEndDate .
    	?incumbency
        	a :Incumbency ;
        	:incumbencyStartDate ?incumbencyStartDate ;
        	:incumbencyEndDate ?incumbencyEndDate .
      }
      WHERE {
      	BIND(@houseid AS ?house)

        ?house a :House ;
    	         :houseName ?houseName .

        OPTIONAL {
          BIND(@partyid AS ?party)

          ?party a :Party .
          ?person a :Member .
    		  ?person :partyMemberHasPartyMembership ?partyMembership .
    		  ?partyMembership :partyMembershipHasParty ?party .
    		  ?party :partyName ?partyName .
          ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .
    	    OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }

    		  ?incumbency :incumbencyHasMember ?person ;
                    	:incumbencyStartDate ?startDate .
          OPTIONAL { ?incumbency :incumbencyEndDate ?incumbencyEndDate . }

          OPTIONAL { ?person :personGivenName ?givenName . }
    	    OPTIONAL { ?person :personFamilyName ?familyName . }
          OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

    			{
    			    ?incumbency :houseIncumbencyHasHouse ?house .
    			}

    			UNION {
        			?incumbency :seatIncumbencyHasHouseSeat ?seat .
        			?seat :houseSeatHasHouse ?house .
    			}
        }
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query.SetUri("partyid", new Uri(BaseController.instance, partyid));

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do match '/parties/:party_id/members/:letter', to: 'houses#party_members_letters', letter: /[A-Za-z]/, via: [:get] end
        [Route(@"{houseid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}/members/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "HousePartyMembersByInitial")]
        [HttpGet]
        public HttpResponseMessage PartyMembers(string houseid, string partyid, string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
      CONSTRUCT {
    	?person
        	a :Person ;
        	:personGivenName ?givenName ;
          :personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
          <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        	:partyMemberHasPartyMembership ?partyMembership ;
        	:memberHasIncumbency ?incumbency .
    	?house
        	a :House ;
        	:houseName ?houseName .
    	?party
        	a :Party ;
        	:partyName ?partyName .
    	?partyMembership
        	a :PartyMembership ;
        	:partyMembershipStartDate ?partyMembershipStartDate ;
        	:partyMembershipEndDate ?partyMembershipEndDate .
    	?incumbency
        	a :Incumbency ;
        	:incumbencyStartDate ?incumbencyStartDate ;
        	:incumbencyEndDate ?incumbencyEndDate .
      }
      WHERE {
      	BIND(@houseid AS ?house)

        ?house a :House ;
    	         :houseName ?houseName .

        OPTIONAL {
          BIND(@partyid AS ?party)

          ?party a :Party .
          ?person a :Member .
    		  ?person :partyMemberHasPartyMembership ?partyMembership .
    		  ?partyMembership :partyMembershipHasParty ?party .
    		  ?party :partyName ?partyName .
          ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .
    	    OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }

    		  ?incumbency :incumbencyHasMember ?person ;
                    	:incumbencyStartDate ?startDate .
          OPTIONAL { ?incumbency :incumbencyEndDate ?incumbencyEndDate . }

          OPTIONAL { ?person :personGivenName ?givenName . }
    	    OPTIONAL { ?person :personFamilyName ?familyName . }
          OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

    			{
    			    ?incumbency :houseIncumbencyHasHouse ?house .
    			}

    			UNION {
        			?incumbency :seatIncumbencyHasHouseSeat ?seat .
        			?seat :houseSeatHasHouse ?house .
    			}
      
    
    FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query.SetUri("partyid", new Uri(BaseController.instance, partyid));
            query.SetLiteral("initial", initial);


            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get 'parties/:party_id/members/a_z_letters', to: 'houses#a_z_letters_party_members' end
        [Route(@"{houseid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}/members/a_z_letters", Name = "HousePartyMembersAToZ")]
        [HttpGet]
        public HttpResponseMessage PartyMembersAToZLetters(string houseid, string partyid)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
         _:x :value ?firstLetter .
      }
      WHERE {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)
          BIND(@partyid AS ?party)

          ?house a :House .
          ?party a :Party .
    	    ?person a :Member .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
    	    ?person :partyMemberHasPartyMembership ?partyMembership .
    	    ?partyMembership :partyMembershipHasParty ?party .
    	    ?incumbency :incumbencyHasMember ?person .

    	    {
    	        ?incumbency :houseIncumbencyHasHouse ?house .
    	    }

    	    UNION {
            	?incumbency :seatIncumbencyHasHouseSeat ?seat .
            	?seat :houseSeatHasHouse ?house .
    	    }

          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query.SetUri("partyid", new Uri(BaseController.instance, partyid));

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get '/parties/:party_id/members/current', to: 'houses#current_party_members' end
        [Route(@"{houseid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}/members/current", Name = "HousePartyCurrentMembers")]
        [HttpGet]
        public HttpResponseMessage PartyCurrentMembers(string houseid, string partyid)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
    	?person
        	a :Person ;
        	:personGivenName ?givenName ;
          :personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
          <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        	:partyMemberHasPartyMembership ?partyMembership ;
        	:memberHasIncumbency ?incumbency .
    	?house
        	a :House ;
        	:houseName ?houseName .
    	?party
        	a :Party ;
        	:partyName ?partyName .
    	?partyMembership
        	a :PartyMembership ;
        	:partyMembershipStartDate ?partyMembershipStartDate .
    	?incumbency
        	a :Incumbency ;
        	:incumbencyStartDate ?incumbencyStartDate .
      }
      WHERE {
      	BIND(@houseid AS ?house)

        ?house a :House ;
    	         :houseName ?houseName .

        OPTIONAL {
          BIND(@partyid AS ?party)

          ?party a :Party .
          ?person a :Member .
    		  ?person :partyMemberHasPartyMembership ?partyMembership .
          FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
    		  ?partyMembership :partyMembershipHasParty ?party .
    		  ?party :partyName ?partyName .
          ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .

    		  ?incumbency :incumbencyHasMember ?person ;
                    	:incumbencyStartDate ?startDate .

          FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }

          OPTIONAL { ?person :personGivenName ?givenName . }
    	    OPTIONAL { ?person :personFamilyName ?familyName . }
          OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

    			{
    			    ?incumbency :houseIncumbencyHasHouse ?house .
    			}

    			UNION {
        			?incumbency :seatIncumbencyHasHouseSeat ?seat .
        			?seat :houseSeatHasHouse ?house .
    			}
        }
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query.SetUri("partyid", new Uri(BaseController.instance, partyid));

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do match '/parties/:party_id/members/current/:letter', to: 'houses#current_party_members_letters', letter: /[A-Za-z]/, via: [:get] end
        [Route(@"{houseid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}/members/current/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "HousePartyCurrentMembersByInitial")]
        [HttpGet]
        public HttpResponseMessage PartyCurrentMembersByInitial(string houseid, string partyid, string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
      CONSTRUCT {
    	?person
        	a :Person ;
        	:personGivenName ?givenName ;
          :personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
          <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        	:partyMemberHasPartyMembership ?partyMembership ;
        	:memberHasIncumbency ?incumbency .
    	?house
        	a :House ;
        	:houseName ?houseName .
    	?party
        	a :Party ;
        	:partyName ?partyName .
    	?partyMembership
        	a :PartyMembership ;
        	:partyMembershipStartDate ?partyMembershipStartDate .
    	?incumbency
        	a :Incumbency ;
        	:incumbencyStartDate ?incumbencyStartDate .
      }
      WHERE {
      	BIND(@houseid AS ?house)

        ?house a :House ;
    	         :houseName ?houseName .

        OPTIONAL {
          BIND(@partyid AS ?party)

          ?party a :Party .
          ?person a :Member .
    		  ?person :partyMemberHasPartyMembership ?partyMembership .
          FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
    		  ?partyMembership :partyMembershipHasParty ?party .
    		  ?party :partyName ?partyName .
          ?partyMembership :partyMembershipStartDate ?partyMembershipStartDate .

    		  ?incumbency :incumbencyHasMember ?person ;
                    	:incumbencyStartDate ?startDate .

          FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }

          OPTIONAL { ?person :personGivenName ?givenName . }
    	    OPTIONAL { ?person :personFamilyName ?familyName . }
          OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

    			{
    			    ?incumbency :houseIncumbencyHasHouse ?house .
    			}

    			UNION {
        			?incumbency :seatIncumbencyHasHouseSeat ?seat .
        			?seat :houseSeatHasHouse ?house .
    			}
        }

    FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
}

";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query.SetUri("partyid", new Uri(BaseController.instance, partyid));
            query.SetLiteral("initial", initial);


            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get 'parties/:party_id/members/current/a_z_letters', to: 'houses#a_z_letters_party_members_current' end
        [Route(@"{houseid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}/members/current/a_z_letters", Name = "HousePartyCurrentMembersAToZ")]
        [HttpGet]
        public HttpResponseMessage PartyCurrentMembersAToZLetters(string houseid, string partyid)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     CONSTRUCT {
         _:x :value ?firstLetter .
      }
      WHERE {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)
          BIND(@partyid AS ?party)

          ?house a :House .
          ?party a :Party .
    	    ?person a :Member .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
    	    ?person :partyMemberHasPartyMembership ?partyMembership .
          FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
    	    ?partyMembership :partyMembershipHasParty ?party .
    	    ?incumbency :incumbencyHasMember ?person .
          FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }

    	    {
    	        ?incumbency :houseIncumbencyHasHouse ?house .
    	    }

    	    UNION {
            	?incumbency :seatIncumbencyHasHouseSeat ?seat .
            	?seat :houseSeatHasHouse ?house .
    	    }

          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query.SetUri("partyid", new Uri(BaseController.instance, partyid));

            return Execute(query);
        }
    }
}
