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
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
         ?houseSeat
         a parl:HouseSeat ;
         parl:houseSeatHasHouse ?house ;
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
         parl:incumbencyStartDate ?seatIncumbencyStartDate ;
         parl:incumbencyEndDate ?seatIncumbencyEndDate .
    	?houseIncumbency
         a parl:HouseIncumbency ;
         parl:houseIncumbencyHasHouse ?house ;
         parl:incumbencyStartDate ?houseIncumbencyStartDate ;
         parl:incumbencyEndDate ?houseIncumbencyEndDate .
        ?house
         a parl:House ;
         parl:houseName ?houseName .
        ?constituencyGroup
        a parl:ConstituencyGroup;
        parl:constituencyGroupName ?constituencyName .
            ?partyMembership
        a parl:PartyMembership ;
        parl:partyMembershipHasParty ?party ;
        parl:partyMembershipStartDate ?partyMembershipStartDate ;
        parl:partyMembershipEndDate ?partyMembershipEndDate .
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
              ?houseIncumbency parl:houseIncumbencyHasHouse ?house .
              ?house parl:houseName ?houseName .
              ?houseIncumbency parl:incumbencyStartDate ?houseIncumbencyStartDate .
              OPTIONAL { ?houseIncumbency parl:incumbencyEndDate ?houseIncumbencyEndDate . }
   		      }
           UNION {
            ?incumbency a parl:SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency)
            ?seatIncumbency parl:seatIncumbencyHasHouseSeat ?houseSeat .
            ?seatIncumbency parl:incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency parl:incumbencyEndDate ?seatIncumbencyEndDate . }
        		?houseSeat parl:houseSeatHasHouse ?house .
            ?house parl:houseName ?houseName .
    				OPTIONAL {
              ?houseSeat parl:houseSeatHasConstituencyGroup ?constituencyGroup .
        			?constituencyGroup parl:constituencyGroupName ?constituencyName .
             }
   				}
    	    ?person parl:partyMemberHasPartyMembership ?partyMembership .
         ?partyMembership parl:partyMembershipHasParty ?party .
         ?party parl:partyName ?partyName .
         ?partyMembership parl:partyMembershipStartDate ?partyMembershipStartDate .
         OPTIONAL { ?partyMembership parl:partyMembershipEndDate ?partyMembershipEndDate . }
         FILTER NOT EXISTS {?incumbency a parl:PastIncumbency}
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
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
         ?houseSeat
         a parl:HouseSeat ;
         parl:houseSeatHasHouse ?house ;
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
         parl:incumbencyStartDate ?seatIncumbencyStartDate ;
         parl:incumbencyEndDate ?seatIncumbencyEndDate .
    	  ?houseIncumbency
         a parl:HouseIncumbency ;
         parl:houseIncumbencyHasHouse ?house ;
         parl:incumbencyStartDate ?houseIncumbencyStartDate ;
         parl:incumbencyEndDate ?houseIncumbencyEndDate .
        ?house
         a parl:House ;
         parl:houseName ?houseName .
        ?constituencyGroup
        a parl:ConstituencyGroup;
        parl:constituencyGroupName ?constituencyName .
        ?partyMembership
        a parl:PartyMembership ;
        parl:partyMembershipHasParty ?party ;
        parl:partyMembershipStartDate ?partyMembershipStartDate ;
        parl:partyMembershipEndDate ?partyMembershipEndDate .
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
            ?houseIncumbency parl:houseIncumbencyHasHouse ?house .
            ?house parl:houseName ?houseName .
            ?houseIncumbency parl:incumbencyStartDate ?houseIncumbencyStartDate .
            OPTIONAL { ?houseIncumbency parl:incumbencyEndDate ?houseIncumbencyEndDate . }
   		    }
         UNION {
          ?incumbency a parl:SeatIncumbency .
          BIND(?incumbency AS ?seatIncumbency)
          ?seatIncumbency parl:seatIncumbencyHasHouseSeat ?houseSeat .
          ?seatIncumbency parl:incumbencyStartDate ?seatIncumbencyStartDate .
          OPTIONAL { ?seatIncumbency parl:incumbencyEndDate ?seatIncumbencyEndDate . }
       	  ?houseSeat parl:houseSeatHasHouse ?house .
          ?house parl:houseName ?houseName .
    		  OPTIONAL { ?houseSeat parl:houseSeatHasConstituencyGroup ?constituencyGroup .
       		?constituencyGroup parl:constituencyGroupName ?constituencyName . }
   		  }

    	  ?person parl:partyMemberHasPartyMembership ?partyMembership .
         ?partyMembership parl:partyMembershipHasParty ?party .
         ?partyMembership parl:partyMembershipStartDate ?partyMembershipStartDate .
         OPTIONAL { ?partyMembership parl:partyMembershipEndDate ?partyMembershipEndDate . }
         ?party parl:partyName ?partyName .


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
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
         _:x parl:value ?firstLetter .
      }
      WHERE {
        SELECT DISTINCT ?firstLetter WHERE {
	        ?Incumbency a parl:Incumbency ;
                        parl:incumbencyHasMember ?person .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
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
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
         ?houseSeat
         a parl:HouseSeat ;
         parl:houseSeatHasHouse ?house ;
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
         parl:incumbencyStartDate ?seatIncumbencyStartDate ;
         parl:incumbencyEndDate ?seatIncumbencyEndDate .
    	?houseIncumbency
         a parl:HouseIncumbency ;
         parl:houseIncumbencyHasHouse ?house ;
         parl:incumbencyStartDate ?houseIncumbencyStartDate ;
         parl:incumbencyEndDate ?houseIncumbencyEndDate .
        ?house
         a parl:House ;
         parl:houseName ?houseName .
        ?constituencyGroup
        a parl:ConstituencyGroup;
        parl:constituencyGroupName ?constituencyName .
            ?partyMembership
        a parl:PartyMembership ;
        parl:partyMembershipHasParty ?party ;
        parl:partyMembershipStartDate ?partyMembershipStartDate ;
        parl:partyMembershipEndDate ?partyMembershipEndDate .
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
              ?houseIncumbency parl:houseIncumbencyHasHouse ?house .
              ?house parl:houseName ?houseName .
              ?houseIncumbency parl:incumbencyStartDate ?houseIncumbencyStartDate .
              OPTIONAL { ?houseIncumbency parl:incumbencyEndDate ?houseIncumbencyEndDate . }
   		   }
           UNION {
            ?incumbency a parl:SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency)
            ?seatIncumbency parl:seatIncumbencyHasHouseSeat ?houseSeat .
            ?seatIncumbency parl:incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency parl:incumbencyEndDate ?seatIncumbencyEndDate . }
        		?houseSeat parl:houseSeatHasHouse ?house .
            ?house parl:houseName ?houseName .
    				OPTIONAL {
              ?houseSeat parl:houseSeatHasConstituencyGroup ?constituencyGroup .
        			?constituencyGroup parl:constituencyGroupName ?constituencyName .
            }
   				}
    	    ?person parl:partyMemberHasPartyMembership ?partyMembership .
         ?partyMembership parl:partyMembershipHasParty ?party .
         ?party parl:partyName ?partyName .
         ?partyMembership parl:partyMembershipStartDate ?partyMembershipStartDate .
         OPTIONAL { ?partyMembership parl:partyMembershipEndDate ?partyMembershipEndDate . }
         FILTER NOT EXISTS {?incumbency a parl:PastIncumbency}

        FILTER STRSTARTS(LCASE(?listAs), LCASE(@letter)) .
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
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
         _:x parl:value ?firstLetter .
      }
      WHERE {
        SELECT DISTINCT ?firstLetter WHERE {
	        ?incumbency a parl:Incumbency ;
          FILTER NOT EXISTS { ?incumbency a parl:PastIncumbency .	}
          ?incumbency parl:incumbencyHasMember ?person .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
";

            var query = new SparqlParameterizedString(queryString);
            return Execute(query);
        }

    }
}

