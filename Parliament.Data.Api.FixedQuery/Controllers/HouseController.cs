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

        [Route("{id:guid}", Name = "HouseById")]
        [HttpGet]
        public HttpResponseMessage ById(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
          ?house
            a parl:House ;
            parl:houseName ?houseName .
      }
      WHERE {
          BIND(@id AS ?house)

          ?house
            a parl:House ;
            parl:houseName ?houseName .
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
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?house
           a parl:House .
      }
      WHERE {
        BIND(@id AS ?id)
        BIND(@source AS ?source)


          ?house a parl:House.
        ?house ?source ?id.
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("source", new Uri(BaseController.schema, source));
            query.SetLiteral("id", id);

            return Execute(query);
        }

        // Ruby route:   get '/houses/:letters', to: 'houses#lookup_by_letters'
        [Route(@"{letters:regex(^\p{L}+$):minlength(2)}", Name = "HouseByLetters")]
        [HttpGet]
        public HttpResponseMessage ByLetters(string letters)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?house
        	a parl:House ;
         	parl:houseName ?houseName .
      }
      WHERE {
        ?house a parl:House .
        ?house parl:houseName ?houseName .


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
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
          ?house
            a parl:House ;
        	  parl:houseName ?houseName .
      }
      WHERE {
          ?house
             a parl:House ;
    			   parl:houseName ?houseName .
      }
";

            var query = new SparqlParameterizedString(queryString);

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get '/members', to: 'houses#members' end
        [Route("{id:guid}/members", Name = "HouseMembers")]
        [HttpGet]
        public HttpResponseMessage Members(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
        	  a parl:Person ;
            parl:personGivenName ?givenName ;
            parl:personFamilyName ?familyName ;
            <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
            <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
            parl:memberHasIncumbency ?incumbency .
    	  ?house
        	  a parl:House ;
        	  parl:houseName ?houseName .
    	  ?incumbency
            a parl:Incumbency ;
        	  parl:incumbencyEndDate ?incumbencyEndDate ;
        	  parl:incumbencyStartDate ?incumbencyStartDate .
      }
      WHERE {
        BIND(@houseid AS ?house)

        ?house a parl:House ;
               parl:houseName ?houseName .
    	  ?person a parl:Member .
    	  ?incumbency parl:incumbencyHasMember ?person ;
       				      parl:incumbencyStartDate ?incumbencyStartDate .

        OPTIONAL { ?incumbency parl:incumbencyEndDate ?incumbencyEndDate . }
        OPTIONAL { ?person parl:personGivenName ?givenName . }
        OPTIONAL { ?person parl:personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

    	  {
    	      ?incumbency parl:houseIncumbencyHasHouse ?house .
    	  }
    	  UNION {
          	?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
          	?seat parl:houseSeatHasHouse ?house .
    	  }
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get '/members/current', to: 'houses#current_members' end
        [Route("{id:guid}/members/current", Name = "HouseCurrentMembers")]
        [HttpGet]
        public HttpResponseMessage CurrentMembers(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
        	  a parl:Person ;
            parl:personGivenName ?givenName ;
            parl:personFamilyName ?familyName ;
            <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
            <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        	  parl:partyMemberHasPartyMembership ?partyMembership ;
            parl:memberHasIncumbency ?incumbency .
    	  ?house
        	  a parl:House ;
        	  parl:houseName ?houseName .
    	  ?seatIncumbency
            a parl:SeatIncumbency ;
        	  parl:incumbencyStartDate ?incumbencyStartDate ;
            parl:seatIncumbencyHasHouseSeat ?seat .
    	  ?houseIncumbency
            a parl:HouseIncumbency ;
        	  parl:incumbencyStartDate ?incumbencyStartDate ;
            parl:houseIncumbencyHasHouse ?house .
        ?seat
            a parl:HouseSeat ;
            parl:houseSeatHasConstituencyGroup ?constituency .
    	  ?partyMembership
        	  a parl:PartyMembership ;
        	  parl:partyMembershipHasParty ?party .
    	  ?party
        	  a parl:Party ;
        	  parl:partyName ?partyName .
        ?constituency
        	a parl:ConstituencyGroup ;
        	parl:constituencyGroupName ?constituencyName .
      }
      WHERE {
        BIND(@houseid AS ?house)

        ?house a parl:House ;
               parl:houseName ?houseName .
    	  ?person a parl:Member .
    	  ?person parl:partyMemberHasPartyMembership ?partyMembership .
    	  FILTER NOT EXISTS { ?partyMembership a parl:PastPartyMembership . }
    	  ?partyMembership parl:partyMembershipHasParty ?party .
    	  ?party parl:partyName ?partyName .
    	  ?incumbency parl:incumbencyHasMember ?person ;
       			       parl:incumbencyStartDate ?incumbencyStartDate .
    	  FILTER NOT EXISTS { ?incumbency a parl:PastIncumbency . }

    	  {
    	      ?incumbency parl:houseIncumbencyHasHouse ?house .
            BIND(?incumbency AS ?houseIncumbency)
    	  }

    	  UNION {
        	?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
        	?seat parl:houseSeatHasHouse ?house .
        	?seat parl:houseSeatHasConstituencyGroup ?constituency .
        	?constituency parl:constituencyGroupName ?constituencyName .
          BIND(?incumbency AS ?seatIncumbency)
    	  }

        OPTIONAL { ?person parl:personGivenName ?givenName . }
        OPTIONAL { ?person parl:personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
      }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, id));

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get '/parties', to: 'houses#parties' end
        [Route("{id:guid}/parties", Name = "HouseParties")]
        [HttpGet]
        public HttpResponseMessage Parties(string id)
        {
            var queryString = @"

PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
       ?house
        	a parl:House ;
        	parl:houseName ?houseName .
        ?party
          a parl:Party ;
          parl:partyName ?partyName .
      }
      WHERE {
        BIND(@houseid as ?house)

        ?house a parl:House ;
               parl:houseName ?houseName .
        ?person a parl:Member .
        ?incumbency parl:incumbencyHasMember ?person ;
                    parl:incumbencyStartDate ?incStartDate .
        OPTIONAL { ?incumbency parl:incumbencyEndDate ?incumbencyEndDate . }

        {
            ?incumbency parl:houseIncumbencyHasHouse ?house .
        }
        UNION
        {
            ?incumbency parl:seatIncumbencyHasHouseSeat ?houseSeat .
            ?houseSeat parl:houseSeatHasHouse ?house .
        }

        ?partyMembership parl:partyMembershipHasPartyMember ?person ;
            			parl:partyMembershipHasParty ?party ;
            			parl:partyMembershipStartDate ?pmStartDate .
        OPTIONAL { ?partyMembership parl:partyMembershipEndDate ?partyMembershipEndDate . }
        ?party parl:partyName ?partyName.

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
        [Route("{id:guid}/parties/current", Name = "HouseCurrentParties")]
        [HttpGet]
        public HttpResponseMessage CurrentParties(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?house
        	a parl:House ;
        	parl:houseName ?houseName .
        ?party
          a parl:Party ;
          parl:partyName ?partyName ;
    	    parl:count ?memberCount .
      }
       WHERE {
        SELECT ?party ?house ?houseName ?partyName (COUNT(?person) AS ?memberCount)
    	  WHERE {

          BIND(@houseid AS ?house)

          ?house a parl:House ;
                 parl:houseName ?houseName .
          ?person a parl:Member .
          ?incumbency parl:incumbencyHasMember ?person .
          FILTER NOT EXISTS { ?incumbency a parl:PastIncumbency . }
    		  ?person parl:partyMemberHasPartyMembership ?partyMembership .
          FILTER NOT EXISTS { ?partyMembership a parl:PastPartyMembership . }
    		  ?partyMembership parl:partyMembershipHasParty ?party .
    		  ?party parl:partyName ?partyName .

    		  {
    		      ?incumbency parl:houseIncumbencyHasHouse ?house .
    		  }

    		  UNION {
          		?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
          		?seat parl:houseSeatHasHouse ?house .
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
        [Route("{houseid:guid}/parties/{partyid:guid}", Name = "HousePartyById")]
        [HttpGet]
        public HttpResponseMessage PartyById(string houseid, string partyid)
        {
            var queryString1 = @"

PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
          ?house
            a parl:House ;
            parl:houseName ?houseName .
          ?party
            a parl:Party ;
            parl:partyName ?partyName .
      }
      WHERE {
          BIND(@houseid AS ?house)

          ?house a parl:House ;
                 parl:houseName ?houseName .

          OPTIONAL {
            BIND(@partyid AS ?party)

            ?party a parl:Party .
    		    ?person a parl:Member .
    		    ?person parl:partyMemberHasPartyMembership ?partyMembership .
    		    ?partyMembership parl:partyMembershipHasParty ?party .
    		    ?party parl:partyName ?partyName .
    		    ?incumbency parl:incumbencyHasMember ?person .

    	      {
    	          ?incumbency parl:houseIncumbencyHasHouse ?house .
    	      }

    	      UNION {
              	?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
              	?seat parl:houseSeatHasHouse ?house .
    	      }
          }
      }

";

            var query1 = new SparqlParameterizedString(queryString1);

            query1.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query1.SetUri("partyid", new Uri(BaseController.instance, partyid));

            var queryString2 = @"

PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
         ?party parl:count ?currentMemberCount .
      }
      WHERE {
    	SELECT ?party (COUNT(?currentMember) AS ?currentMemberCount) WHERE {
          BIND(@houseid AS ?house)
          BIND(@partyid AS ?party)

          ?house a parl:House .
          ?party a parl:Party .

          OPTIONAL {
    	      ?party parl:partyHasPartyMembership ?partyMembership .
    	      FILTER NOT EXISTS { ?partyMembership a parl:PastPartyMembership . }
    	      ?partyMembership parl:partyMembershipHasPartyMember ?currentMember .
    	      ?currentMember parl:memberHasIncumbency ?incumbency .
    	      FILTER NOT EXISTS { ?incumbency a parl:PastIncumbency . }

            {
    	          ?incumbency parl:houseIncumbencyHasHouse ?house .
    	      }

    	      UNION {
              	?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
              	?seat parl:houseSeatHasHouse ?house .
    	      }
          }
        }
        GROUP BY ?party
      }

";

            var query2 = new SparqlParameterizedString(queryString2);

            query2.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query2.SetUri("partyid", new Uri(BaseController.instance, partyid));

            return Execute(query1.ToString(), query2.ToString());


        }

        // Ruby route: resources :houses, only: [:index] do match '/members/:letter', to: 'houses#members_letters', letter: /[A-Za-z]/, via: [:get] end
        [Route("{houseid:guid}/members/{initial:maxlength(1)}", Name = "HouseMembersByInitial")]
        [HttpGet]
        public HttpResponseMessage MembersByInitial(string houseid, string initial)
        {
            var queryString = @"

PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
        	  a parl:Person ;
            parl:personGivenName ?givenName ;
            parl:personFamilyName ?familyName ;
            <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
            <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
            parl:memberHasIncumbency ?incumbency .
    	  ?house
        	  a parl:House ;
        	  parl:houseName ?houseName .
    	  ?incumbency
            a parl:Incumbency ;
        	  parl:incumbencyEndDate ?incumbencyEndDate ;
        	  parl:incumbencyStartDate ?incumbencyStartDate .
      }
      WHERE {
        BIND(@houseid AS ?house)

        ?house a parl:House ;
               parl:houseName ?houseName .
    	  ?person a parl:Member .
    	  ?incumbency parl:incumbencyHasMember ?person ;
       				      parl:incumbencyStartDate ?incumbencyStartDate .

        OPTIONAL { ?incumbency parl:incumbencyEndDate ?incumbencyEndDate . }
        OPTIONAL { ?person parl:personGivenName ?givenName . }
        OPTIONAL { ?person parl:personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

    	  {
    	      ?incumbency parl:houseIncumbencyHasHouse ?house .
    	  }
    	  UNION {
          	?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
          	?seat parl:houseSeatHasHouse ?house .
    	  }

        FILTER STRSTARTS(LCASE(?familyName), LCASE(@initial))
}

";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query.SetLiteral("initial", initial);

            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get '/members/a_z_letters', to: 'houses#a_z_letters_members' end
        [Route("{id:guid}/members/a_z_letters", Name = "HouseMembersAToZ")]
        [HttpGet]
        public HttpResponseMessage MembersAToZLetters(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
         _:x parl:value ?firstLetter .
      }
      WHERE {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)

          ?house a parl:House ;
	               parl:houseName ?houseName .
    	    ?person a parl:Member ;
                  <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
    	    ?incumbency parl:incumbencyHasMember ?person .

    	    {
    	        ?incumbency parl:houseIncumbencyHasHouse ?house .
    	    }

    	    UNION {
            	?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
            	?seat parl:houseSeatHasHouse ?house .
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
        [Route("{houseid:guid}/members/current/{initial:maxlength(1)}", Name = "HouseCurrentMembersByInitial")]
        [HttpGet]
        public HttpResponseMessage CurrentMembersByInitial(string houseid, string initial)
        {
            var queryString = @"

PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
        ?person
        	  a parl:Person ;
            parl:personGivenName ?givenName ;
            parl:personFamilyName ?familyName ;
            <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
            <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        	  parl:partyMemberHasPartyMembership ?partyMembership ;
            parl:memberHasIncumbency ?incumbency .
    	  ?house
        	  a parl:House ;
        	  parl:houseName ?houseName .
    	  ?seatIncumbency
            a parl:SeatIncumbency ;
        	  parl:incumbencyStartDate ?incumbencyStartDate ;
            parl:seatIncumbencyHasHouseSeat ?seat .
    	  ?houseIncumbency
            a parl:HouseIncumbency ;
        	  parl:incumbencyStartDate ?incumbencyStartDate ;
            parl:houseIncumbencyHasHouse ?house .
        ?seat
            a parl:HouseSeat ;
            parl:houseSeatHasConstituencyGroup ?constituency .
    	  ?partyMembership
        	  a parl:PartyMembership ;
        	  parl:partyMembershipHasParty ?party .
    	  ?party
        	  a parl:Party ;
        	  parl:partyName ?partyName .
        ?constituency
        	a parl:ConstituencyGroup ;
        	parl:constituencyGroupName ?constituencyName .
      }
      WHERE {
        BIND(@houseid AS ?house)

        ?house a parl:House ;
               parl:houseName ?houseName .
    	  ?person a parl:Member .
    	  ?person parl:partyMemberHasPartyMembership ?partyMembership .
    	  FILTER NOT EXISTS { ?partyMembership a parl:PastPartyMembership . }
    	  ?partyMembership parl:partyMembershipHasParty ?party .
    	  ?party parl:partyName ?partyName .
    	  ?incumbency parl:incumbencyHasMember ?person ;
       			       parl:incumbencyStartDate ?incumbencyStartDate .
    	  FILTER NOT EXISTS { ?incumbency a parl:PastIncumbency . }

    	  {
    	      ?incumbency parl:houseIncumbencyHasHouse ?house .
            BIND(?incumbency AS ?houseIncumbency)
    	  }

    	  UNION {
        	?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
        	?seat parl:houseSeatHasHouse ?house .
        	?seat parl:houseSeatHasConstituencyGroup ?constituency .
        	?constituency parl:constituencyGroupName ?constituencyName .
          BIND(?incumbency AS ?seatIncumbency)
    	  }

        OPTIONAL { ?person parl:personGivenName ?givenName . }
        OPTIONAL { ?person parl:personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

        FILTER STRSTARTS(LCASE(?familyName), LCASE(@initial))
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query.SetLiteral("initial", initial);

            return Execute(query);
        }
        // Ruby route: resources :houses, only: [:index] do get '/members/current/a_z_letters', to: 'houses#a_z_letters_members' end
        [Route("{id:guid}/members/current/a_z_letters", Name = "HouseCurrentMembersAToZ")]
        [HttpGet]
        public HttpResponseMessage CurrentMembersAToZLetters(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
         _:x parl:value ?firstLetter .
      }
      WHERE {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)

          ?house a parl:House ;
	               parl:houseName ?houseName .
    	    ?person a parl:Member;
       			      <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
    	    ?incumbency parl:incumbencyHasMember ?person .
    	    FILTER NOT EXISTS { ?incumbency a parl:PastIncumbency . }

    	    {
    	        ?incumbency parl:houseIncumbencyHasHouse ?house .
    	    }

    	    UNION {
            	?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
            	?seat parl:houseSeatHasHouse ?house .
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
        [Route("{houseid:guid}/parties/{partyid:guid}/members", Name = "HousePartyMembers")]
        [HttpGet]
        public HttpResponseMessage PartyMembers(string houseid, string partyid)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
    	?person
        	a parl:Person ;
        	parl:personGivenName ?givenName ;
          parl:personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
          <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        	parl:partyMemberHasPartyMembership ?partyMembership ;
        	parl:memberHasIncumbency ?incumbency .
    	?house
        	a parl:House ;
        	parl:houseName ?houseName .
    	?party
        	a parl:Party ;
        	parl:partyName ?partyName .
    	?partyMembership
        	a parl:PartyMembership ;
        	parl:partyMembershipStartDate ?partyMembershipStartDate ;
        	parl:partyMembershipEndDate ?partyMembershipEndDate .
    	?incumbency
        	a parl:Incumbency ;
        	parl:incumbencyStartDate ?incumbencyStartDate ;
        	parl:incumbencyEndDate ?incumbencyEndDate .
      }
      WHERE {
      	BIND(@houseid AS ?house)

        ?house a parl:House ;
    	         parl:houseName ?houseName .

        OPTIONAL {
          BIND(@partyid AS ?party)

          ?party a parl:Party .
          ?person a parl:Member .
    		  ?person parl:partyMemberHasPartyMembership ?partyMembership .
    		  ?partyMembership parl:partyMembershipHasParty ?party .
    		  ?party parl:partyName ?partyName .
          ?partyMembership parl:partyMembershipStartDate ?partyMembershipStartDate .
    	    OPTIONAL { ?partyMembership parl:partyMembershipEndDate ?partyMembershipEndDate . }

    		  ?incumbency parl:incumbencyHasMember ?person ;
                    	parl:incumbencyStartDate ?startDate .
          OPTIONAL { ?incumbency parl:incumbencyEndDate ?incumbencyEndDate . }

          OPTIONAL { ?person parl:personGivenName ?givenName . }
    	    OPTIONAL { ?person parl:personFamilyName ?familyName . }
          OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

    			{
    			    ?incumbency parl:houseIncumbencyHasHouse ?house .
    			}

    			UNION {
        			?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
        			?seat parl:houseSeatHasHouse ?house .
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
        [Route(@"{houseid:guid}/parties/{partyid:guid}/members/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "HousePartyMembersByInitial")]
        [HttpGet]
        public HttpResponseMessage PartyMembers(string houseid, string partyid, string initial)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
      CONSTRUCT {
    	?person
        	a parl:Person ;
        	parl:personGivenName ?givenName ;
          parl:personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
          <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        	parl:partyMemberHasPartyMembership ?partyMembership ;
        	parl:memberHasIncumbency ?incumbency .
    	?house
        	a parl:House ;
        	parl:houseName ?houseName .
    	?party
        	a parl:Party ;
        	parl:partyName ?partyName .
    	?partyMembership
        	a parl:PartyMembership ;
        	parl:partyMembershipStartDate ?partyMembershipStartDate ;
        	parl:partyMembershipEndDate ?partyMembershipEndDate .
    	?incumbency
        	a parl:Incumbency ;
        	parl:incumbencyStartDate ?incumbencyStartDate ;
        	parl:incumbencyEndDate ?incumbencyEndDate .
      }
      WHERE {
      	BIND(@houseid AS ?house)

        ?house a parl:House ;
    	         parl:houseName ?houseName .

        OPTIONAL {
          BIND(@partyid AS ?party)

          ?party a parl:Party .
          ?person a parl:Member .
    		  ?person parl:partyMemberHasPartyMembership ?partyMembership .
    		  ?partyMembership parl:partyMembershipHasParty ?party .
    		  ?party parl:partyName ?partyName .
          ?partyMembership parl:partyMembershipStartDate ?partyMembershipStartDate .
    	    OPTIONAL { ?partyMembership parl:partyMembershipEndDate ?partyMembershipEndDate . }

    		  ?incumbency parl:incumbencyHasMember ?person ;
                    	parl:incumbencyStartDate ?startDate .
          OPTIONAL { ?incumbency parl:incumbencyEndDate ?incumbencyEndDate . }

          OPTIONAL { ?person parl:personGivenName ?givenName . }
    	    OPTIONAL { ?person parl:personFamilyName ?familyName . }
          OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

    			{
    			    ?incumbency parl:houseIncumbencyHasHouse ?house .
    			}

    			UNION {
        			?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
        			?seat parl:houseSeatHasHouse ?house .
    			}
      
    
    FILTER STRSTARTS(LCASE(?familyName), LCASE(@initial))
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
        [Route("{houseid:guid}/parties/{partyid:guid}/members/a_z_letters", Name = "HousePartyMembersAToZ")]
        [HttpGet]
        public HttpResponseMessage PartyMembersAToZLetters(string houseid, string partyid)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
         _:x parl:value ?firstLetter .
      }
      WHERE {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)
          BIND(@partyid AS ?party)

          ?house a parl:House .
          ?party a parl:Party .
    	    ?person a parl:Member .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
    	    ?person parl:partyMemberHasPartyMembership ?partyMembership .
    	    ?partyMembership parl:partyMembershipHasParty ?party .
    	    ?incumbency parl:incumbencyHasMember ?person .

    	    {
    	        ?incumbency parl:houseIncumbencyHasHouse ?house .
    	    }

    	    UNION {
            	?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
            	?seat parl:houseSeatHasHouse ?house .
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
        [Route("{houseid:guid}/parties/{partyid:guid}/members/current", Name = "HousePartyCurrentMembers")]
        [HttpGet]
        public HttpResponseMessage PartyCurrentMembers(string houseid, string partyid)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
    	?person
        	a parl:Person ;
        	parl:personGivenName ?givenName ;
          parl:personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
          <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        	parl:partyMemberHasPartyMembership ?partyMembership ;
        	parl:memberHasIncumbency ?incumbency .
    	?house
        	a parl:House ;
        	parl:houseName ?houseName .
    	?party
        	a parl:Party ;
        	parl:partyName ?partyName .
    	?partyMembership
        	a parl:PartyMembership ;
        	parl:partyMembershipStartDate ?partyMembershipStartDate .
    	?incumbency
        	a parl:Incumbency ;
        	parl:incumbencyStartDate ?incumbencyStartDate .
      }
      WHERE {
      	BIND(@houseid AS ?house)

        ?house a parl:House ;
    	         parl:houseName ?houseName .

        OPTIONAL {
          BIND(@partyid AS ?party)

          ?party a parl:Party .
          ?person a parl:Member .
    		  ?person parl:partyMemberHasPartyMembership ?partyMembership .
          FILTER NOT EXISTS { ?partyMembership a parl:PastPartyMembership . }
    		  ?partyMembership parl:partyMembershipHasParty ?party .
    		  ?party parl:partyName ?partyName .
          ?partyMembership parl:partyMembershipStartDate ?partyMembershipStartDate .

    		  ?incumbency parl:incumbencyHasMember ?person ;
                    	parl:incumbencyStartDate ?startDate .

          FILTER NOT EXISTS { ?incumbency a parl:PastIncumbency . }

          OPTIONAL { ?person parl:personGivenName ?givenName . }
    	    OPTIONAL { ?person parl:personFamilyName ?familyName . }
          OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

    			{
    			    ?incumbency parl:houseIncumbencyHasHouse ?house .
    			}

    			UNION {
        			?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
        			?seat parl:houseSeatHasHouse ?house .
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
        [Route(@"{houseid:guid}/parties/{partyid:guid}/members/current/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "HousePartyCurrentMembersByInitial")]
        [HttpGet]
        public HttpResponseMessage PartyCurrentMembersByInitial(string houseid, string partyid, string initial)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
      CONSTRUCT {
    	?person
        	a parl:Person ;
        	parl:personGivenName ?givenName ;
          parl:personFamilyName ?familyName ;
          <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
          <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        	parl:partyMemberHasPartyMembership ?partyMembership ;
        	parl:memberHasIncumbency ?incumbency .
    	?house
        	a parl:House ;
        	parl:houseName ?houseName .
    	?party
        	a parl:Party ;
        	parl:partyName ?partyName .
    	?partyMembership
        	a parl:PartyMembership ;
        	parl:partyMembershipStartDate ?partyMembershipStartDate .
    	?incumbency
        	a parl:Incumbency ;
        	parl:incumbencyStartDate ?incumbencyStartDate .
      }
      WHERE {
      	BIND(@houseid AS ?house)

        ?house a parl:House ;
    	         parl:houseName ?houseName .

        OPTIONAL {
          BIND(@partyid AS ?party)

          ?party a parl:Party .
          ?person a parl:Member .
    		  ?person parl:partyMemberHasPartyMembership ?partyMembership .
          FILTER NOT EXISTS { ?partyMembership a parl:PastPartyMembership . }
    		  ?partyMembership parl:partyMembershipHasParty ?party .
    		  ?party parl:partyName ?partyName .
          ?partyMembership parl:partyMembershipStartDate ?partyMembershipStartDate .

    		  ?incumbency parl:incumbencyHasMember ?person ;
                    	parl:incumbencyStartDate ?startDate .

          FILTER NOT EXISTS { ?incumbency a parl:PastIncumbency . }

          OPTIONAL { ?person parl:personGivenName ?givenName . }
    	    OPTIONAL { ?person parl:personFamilyName ?familyName . }
          OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

    			{
    			    ?incumbency parl:houseIncumbencyHasHouse ?house .
    			}

    			UNION {
        			?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
        			?seat parl:houseSeatHasHouse ?house .
    			}
        }

    FILTER STRSTARTS(LCASE(?familyName), LCASE(@initial))
}

";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
            query.SetUri("partyid", new Uri(BaseController.instance, partyid));
            query.SetLiteral("initial", initial);


            return Execute(query);
        }

        // Ruby route: resources :houses, only: [:index] do get 'parties/:party_id/members/current/a_z_letters', to: 'houses#a_z_letters_party_members_current' end
        [Route("{houseid:guid}/parties/{partyid:guid}/members/current/a_z_letters", Name = "HousePartyCurrentMembersAToZ")]
        [HttpGet]
        public HttpResponseMessage PartyCurrentMembersAToZLetters(string houseid, string partyid)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
         _:x parl:value ?firstLetter .
      }
      WHERE {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)
          BIND(@partyid AS ?party)

          ?house a parl:House .
          ?party a parl:Party .
    	    ?person a parl:Member .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
    	    ?person parl:partyMemberHasPartyMembership ?partyMembership .
          FILTER NOT EXISTS { ?partyMembership a parl:PastPartyMembership . }
    	    ?partyMembership parl:partyMembershipHasParty ?party .
    	    ?incumbency parl:incumbencyHasMember ?person .
          FILTER NOT EXISTS { ?incumbency a parl:PastIncumbency . }

    	    {
    	        ?incumbency parl:houseIncumbencyHasHouse ?house .
    	    }

    	    UNION {
            	?incumbency parl:seatIncumbencyHasHouseSeat ?seat .
            	?seat parl:houseSeatHasHouse ?house .
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
