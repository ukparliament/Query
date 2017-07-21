namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController 
    {
        //[Route("", Name = "ParliamentIndex")]
        [HttpGet]
        public Graph parliament_index()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?parliament
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate ;
        :parliamentPeriodEndDate ?endDate ;
        :parliamentPeriodNumber ?parliamentNumber .
}
WHERE {
    ?parliament 
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate ;
        :parliamentPeriodNumber ?parliamentNumber .
    OPTIONAL { ?parliament :parliamentPeriodEndDate ?endDate . }
}
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        //[Route("current", Name = "ParliamentCurrent")]
        [HttpGet]
        public Graph parliament_current()
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?parliament
        a :ParliamentPeriod .
}
WHERE {
    ?parliament 
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate .
    FILTER NOT EXISTS { ?parliament a :PastParliamentPeriod }
    BIND(xsd:dateTime(?startDate) AS ?startDateTime)
    BIND(now() AS ?currentDate)
    FILTER(?startDateTime < ?currentDate)
}
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteSingle(query);
        }

        //[Route("previous", Name = "ParliamentPrevious")]
        [HttpGet]
        public Graph parliament_previous()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?previousParliament
        a :ParliamentPeriod .
}
WHERE {
    {
        ?parliament a :ParliamentPeriod .
        FILTER NOT EXISTS { ?parliament a :PastParliamentPeriod }
        ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament .

    }
    UNION {
        ?parliament a :ParliamentPeriod .
        {
            SELECT (max(?parliamentPeriodEndDate) AS ?maxEndDate) 
            WHERE {
                ?parliament 
                    a :ParliamentPeriod ;
                    :parliamentPeriodEndDate ?parliamentPeriodEndDate .
            }
   		}
        ?parliament :parliamentPeriodEndDate ?maxEndDate .
        BIND(?parliament AS ?previousParliament)
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        //[Route("next", Name = "ParliamentNext")]
        [HttpGet]
        public Graph parliament_next()
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?nextParliament
        a :ParliamentPeriod .
}
WHERE {
    ?nextParliament 
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate .
    BIND(now() AS ?currentDate)
    BIND(xsd:dateTime(?startDate) AS ?startDateTime)
    FILTER(?startDateTime > ?currentDate)
}";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"lookup/{source:regex(^\w+$)}/{id}", Name = "ParliamentLookup")]
        [HttpGet]
        public Graph parliament_lookup(string property, string value) => base.LookupInternal("ParliamentPeriod", property, value);

        //[Route(@"{id:regex(^\w{8}$)}", Name = "ParliamentById")]
        [HttpGet]
        public Graph parliament_by_id(string parliament_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?parliament
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate ;
        :parliamentPeriodEndDate ?endDate ;
        :parliamentPeriodNumber ?parliamentNumber ;
        :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	:parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament .
    ?party
        a :Party ;
        :partyName ?partyName ;
        :count ?memberCount .
}
WHERE {
    SELECT ?parliament ?startDate ?endDate ?parliamentNumber ?party ?partyName ?nextParliament ?previousParliament (COUNT(?member) AS ?memberCount)
    WHERE {
        BIND(@id AS ?parliament)
        ?parliament
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?startDate ;
            :parliamentPeriodNumber ?parliamentNumber .
            OPTIONAL { ?parliament :parliamentPeriodEndDate ?endDate . }
        	OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
            OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
            OPTIONAL {
                ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
                ?seatIncumbency 
                    :incumbencyStartDate ?incStartDate ;                
           			:incumbencyHasMember ?member .
                OPTIONAL { ?seatIncumbency :incumbencyEndDate ?incumbencyEndDate . }
                ?member :partyMemberHasPartyMembership ?partyMembership .
                ?partyMembership 
                    :partyMembershipHasParty ?party ;
                    :partyMembershipStartDate ?pmStartDate .
                OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
                ?party :partyName ?partyName .
                BIND(COALESCE(?partyMembershipEndDate,now()) AS ?pmEndDate)
    		    BIND(COALESCE(?incumbencyEndDate,now()) AS ?incEndDate)
                FILTER (
                    (?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
                    (?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
                )
            }
        }
	GROUP BY ?parliament ?startDate ?endDate ?parliamentNumber ?party ?partyName ?nextParliament ?previousParliament
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteSingle(query);

        }

        //[Route(@"{id:regex(^\w{8}$)}/next", Name = "NextParliamentById")]
        [HttpGet]
        public Graph next_parliament_by_id(string parliament_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?nextParliament
        a :ParliamentPeriod .
}
WHERE {
    BIND(@id AS ?parliament)
    
    ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament .
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteSingle(query);

        }

        //[Route(@"{id:regex(^\w{8}$)}/previous", Name = "PreviousParliamentById")]
        [HttpGet]
        public Graph previous_parliament_by_id(string parliament_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?previousParliament
        a :ParliamentPeriod .
}
WHERE {
    BIND(@id AS ?parliament)
    
    ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament .
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteSingle(query);

        }

        //[Route(@"{id:regex(^\w{8}$)}/members", Name = "ParliamentMembers")]
        [HttpGet]
        public Graph parliament_members(string parliament_id)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?seatIncumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?incStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .   
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?pmStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
     ?parliament 
         a :ParliamentPeriod ;
         :parliamentPeriodStartDate ?parliamentStartDate ;
         :parliamentPeriodEndDate ?parliamentEndDate ;
         :parliamentPeriodNumber ?parliamentNumber ;
         :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	 :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament ;
         :count ?memberCount .
    ?house
        a :House ;
        :houseName ?houseName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@parliamentid AS ?parliament)
        ?parliament 
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?parliamentStartDate ;
            :parliamentPeriodNumber ?parliamentNumber .
        OPTIONAL { ?parliament :parliamentPeriodEndDate ?parliamentEndDate . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
        
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
            ?seatIncumbency :incumbencyHasMember ?person ;
                            :seatIncumbencyHasHouseSeat ?houseSeat ;
                			:incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup ;
                       :houseSeatHasHouse ?house .
            ?house :houseName ?houseName .
            ?constituencyGroup :constituencyGroupName ?constituencyName .

            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
                
            ?person :partyMemberHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasParty ?party ;
                                :partyMembershipStartDate ?partyMembershipStartDate .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }               
            ?party :partyName ?partyName .

            BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)
                
            BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
            BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
            FILTER (
                (?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
                (?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
            )
        }
    }
}
UNION {
    SELECT ?parliament (COUNT(?person) AS ?memberCount) WHERE {
        BIND(@parliamentid AS ?parliament)
        ?parliament a :ParliamentPeriod .
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
         	?seatIncumbency :incumbencyHasMember ?person .
        }
      }
    GROUP BY ?parliament
    }
UNION {
    SELECT DISTINCT ?firstLetter WHERE {
        BIND(@parliamentid AS ?parliament)

        ?parliament a :ParliamentPeriod ;                
        			:parliamentPeriodHasSeatIncumbency ?seatIncumbency.
        ?seatIncumbency :incumbencyHasMember ?person .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
    }
  }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ParliamentMembersByInitial")]
        [HttpGet]
        public Graph parliament_members_by_initial(string parliament_id, string initial)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?seatIncumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?incStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .   
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?pmStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
     ?parliament 
         a :ParliamentPeriod ;
         :parliamentPeriodStartDate ?parliamentStartDate ;
         :parliamentPeriodEndDate ?parliamentEndDate ;
         :parliamentPeriodNumber ?parliamentNumber ;
         :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	 :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament ;
         :count ?memberCount .
    ?house
        a :House ;
        :houseName ?houseName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@parliamentid AS ?parliament)
        ?parliament 
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?parliamentStartDate ;
            :parliamentPeriodNumber ?parliamentNumber .
        OPTIONAL { ?parliament :parliamentPeriodEndDate ?parliamentEndDate . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
        
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
            ?seatIncumbency :incumbencyHasMember ?person ;
                            :seatIncumbencyHasHouseSeat ?houseSeat ;
                			:incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup ;
                       :houseSeatHasHouse ?house .
            ?house :houseName ?houseName .
            ?constituencyGroup :constituencyGroupName ?constituencyName .

            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
                
                ?person :partyMemberHasPartyMembership ?partyMembership .
                ?partyMembership :partyMembershipHasParty ?party ;
                                 :partyMembershipStartDate ?partyMembershipStartDate .
                OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }               
                ?party :partyName ?partyName .
                
                 BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        		 BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        		 BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        		 BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)

                BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
                BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
                FILTER (
                    (?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
                    (?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
                )
        FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
        }
    }
}
UNION {
	SELECT ?parliament (COUNT(DISTINCT(?person)) AS ?memberCount) WHERE {
        BIND(@parliamentid AS ?parliament)
        ?parliament a :ParliamentPeriod .
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
         	?seatIncumbency :incumbencyHasMember ?person .
        }
      }
    GROUP BY ?parliament
    }
UNION {
    SELECT DISTINCT ?firstLetter WHERE {
        BIND(@parliamentid AS ?parliament)

        ?parliament a :ParliamentPeriod ;                
        			:parliamentPeriodHasSeatIncumbency ?seatIncumbency.
        ?seatIncumbency :incumbencyHasMember ?person .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
    }
  }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members/a_z_letters", Name = "ParliamentMembersAToZLetters")]
        [HttpGet]
        public Graph parliament_members_a_to_z(string parliament_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    _:x :value ?firstLetter .
}
WHERE {
    SELECT DISTINCT ?firstLetter WHERE {
        BIND(@parliamentid AS ?parliament)

        ?parliament a :ParliamentPeriod ;                
        			:parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :incumbencyHasMember ?person .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
    }
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/houses", Name = "ParliamentHouses")]
        [HttpGet]
        public Graph parliament_houses(string parliament_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
	 ?house
        a :House ;
        :houseName ?houseName .
     ?parliament 
         a :ParliamentPeriod ;
         :parliamentPeriodStartDate ?parliamentStartDate ;
         :parliamentPeriodEndDate ?parliamentEndDate ;
         :parliamentPeriodNumber ?parliamentNumber ;
         :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	 :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament .
}
WHERE {
    BIND(@parliamentid AS ?parliament)
    ?parliament 
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?parliamentStartDate ;
        :parliamentPeriodNumber ?parliamentNumber .
    OPTIONAL { ?parliament :parliamentPeriodEndDate ?parliamentEndDate . }
    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }

    OPTIONAL {
        ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :incumbencyHasMember ?person ;
                        :seatIncumbencyHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasHouse ?house .
        ?house :houseName ?houseName . 
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/houses/{houseid:regex(^\w{8}$)}", Name = "ParliamentHouse")]
        [HttpGet]
        public Graph parliament_house(string parliament_id, string house_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
	 ?house
        a :House ;
        :houseName ?houseName .
     ?parliament 
         a :ParliamentPeriod ;
         :parliamentPeriodStartDate ?parliamentStartDate ;
         :parliamentPeriodEndDate ?parliamentEndDate ;
         :parliamentPeriodNumber ?parliamentNumber ;
         :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	 :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament .
}
WHERE {
    BIND(@parliamentid AS ?parliament)

    ?parliament 
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?parliamentStartDate ;
        :parliamentPeriodNumber ?parliamentNumber .
    OPTIONAL { ?parliament :parliamentPeriodEndDate ?parliamentEndDate . }
    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }

    OPTIONAL {    
        BIND(@houseid AS ?house)
        
        ?house
            a :House ;
            :houseName ?houseName .
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
            ?seatIncumbency :incumbencyHasMember ?person ;
                            :seatIncumbencyHasHouseSeat ?houseSeat .
            ?houseSeat :houseSeatHasHouse ?house .
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteSingle(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/houses/{houseid:regex(^\w{8}$)}/members", Name = "ParliamentHouseMembers")]
        [HttpGet]
        public Graph parliament_house_members(string parliament_id, string house_id)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?seatIncumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?incStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .   
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?pmStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
     ?parliament 
         a :ParliamentPeriod ;
         :parliamentPeriodStartDate ?parliamentStartDate ;
         :parliamentPeriodEndDate ?parliamentEndDate ;
         :parliamentPeriodNumber ?parliamentNumber ;
         :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	 :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament ;
         :count ?memberCount .
    ?house
        a :House ;
        :houseName ?houseName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
          BIND(@parliamentid AS ?parliament)
		  BIND(@houseid AS ?house)
        ?parliament 
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?parliamentStartDate ;
            :parliamentPeriodNumber ?parliamentNumber .
        ?house
            a :House ;
            :houseName ?houseName .
        OPTIONAL { ?parliament :parliamentPeriodEndDate ?parliamentEndDate . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
        
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
            ?seatIncumbency :incumbencyHasMember ?person ;
                            :incumbencyStartDate ?seatIncumbencyStartDate ;
                            :seatIncumbencyHasHouseSeat ?houseSeat .
            ?houseSeat :houseSeatHasHouse ?house .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
            ?constituencyGroup :constituencyGroupName ?constituencyName .

            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

                ?person :partyMemberHasPartyMembership ?partyMembership .
                ?partyMembership :partyMembershipHasParty ?party ;
                                 :partyMembershipStartDate ?partyMembershipStartDate .
                OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }               
                ?party :partyName ?partyName .

                BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
                BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
                BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
                BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)
                
                BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
                BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
                FILTER (
                    (?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
                    (?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
                )
          }
       }
    }
    UNION {
	SELECT ?parliament (COUNT(DISTINCT(?person)) AS ?memberCount) WHERE {
          BIND(@parliamentid AS ?parliament)
		  BIND(@houseid AS ?house)

        ?parliament a :ParliamentPeriod .
        ?house a :House .
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
         	?seatIncumbency :incumbencyHasMember ?person ;
                          	:seatIncumbencyHasHouseSeat ?houseSeat .
            ?houseSeat :houseSeatHasHouse ?house .
        }
      }
    GROUP BY ?parliament
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@parliamentid AS ?parliament)
		  BIND(@houseid AS ?house)
            
          ?parliament a :ParliamentPeriod .
          ?house a :House .
       	  ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
          ?seatIncumbency :incumbencyHasMember ?person ;
          				  :seatIncumbencyHasHouseSeat ?houseSeat .
          ?houseSeat :houseSeatHasHouse ?house .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/houses/{houseid:regex(^\w{8}$)}/members/a_z_letters", Name = "ParliamentHouseMembersAToZLetters")]
        [HttpGet]
        public Graph parliament_house_members_a_to_z(string parliament_id, string house_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    _:x :value ?firstLetter .
}
WHERE {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@parliamentid AS ?parliament)
		  BIND(@houseid AS ?house)
            
          ?parliament a :ParliamentPeriod .
          ?house a :House .
       	  ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
          ?seatIncumbency :incumbencyHasMember ?person ;
          				  :seatIncumbencyHasHouseSeat ?houseSeat .
          ?houseSeat :houseSeatHasHouse ?house .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/houses/{houseid:regex(^\w{8}$)}/members/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ParliamentHouseMembersByInitial")]
        [HttpGet]
        public Graph parliament_house_members_by_initial(string parliament_id, string house_id, string initial)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?seatIncumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?incStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .   
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?pmStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
     ?parliament 
         a :ParliamentPeriod ;
         :parliamentPeriodStartDate ?parliamentStartDate ;
         :parliamentPeriodEndDate ?parliamentEndDate ;
         :parliamentPeriodNumber ?parliamentNumber ;
         :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	 :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament ;
         :count ?memberCount .
    ?house
        a :House ;
        :houseName ?houseName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
          BIND(@parliamentid AS ?parliament)
          BIND(@houseid AS ?house)
        ?parliament 
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?parliamentStartDate ;
            :parliamentPeriodNumber ?parliamentNumber .
        ?house
            a :House ;
            :houseName ?houseName .
        OPTIONAL { ?parliament :parliamentPeriodEndDate ?parliamentEndDate . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
        
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
            ?seatIncumbency :incumbencyHasMember ?person ;
                            :incumbencyStartDate ?seatIncumbencyStartDate ;
                            :seatIncumbencyHasHouseSeat ?houseSeat .
            ?houseSeat :houseSeatHasHouse ?house .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
            ?constituencyGroup :constituencyGroupName ?constituencyName .

            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

            ?person :partyMemberHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasParty ?party ;
                                :partyMembershipStartDate ?partyMembershipStartDate .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }               
            ?party :partyName ?partyName .

            BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)
            
            BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
            BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
            FILTER (
                (?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
                (?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
            )
                FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
          }
       }
    }
    UNION {
	SELECT ?parliament (COUNT(DISTINCT(?person)) AS ?memberCount) WHERE {
          BIND(@parliamentid AS ?parliament)
          BIND(@houseid AS ?house)

        ?parliament a :ParliamentPeriod .
        ?house a :House .
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
         	?seatIncumbency :incumbencyHasMember ?person ;
                          	:seatIncumbencyHasHouseSeat ?houseSeat .
            ?houseSeat :houseSeatHasHouse ?house .
        }
      }
    GROUP BY ?parliament
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@parliamentid AS ?parliament)
          BIND(@houseid AS ?house)
            
          ?parliament a :ParliamentPeriod.
          ?house a :House.
          ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
          ?seatIncumbency :incumbencyHasMember ?person ;
          				  :seatIncumbencyHasHouseSeat ?houseSeat.
          ?houseSeat :houseSeatHasHouse ?house .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/parties", Name = "ParliamentParties")]
        [HttpGet]
        public Graph parliament_parties(string parliament_id)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?parliament
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate ;
        :parliamentPeriodEndDate ?endDate ;
        :parliamentPeriodNumber ?parliamentNumber ;
        :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	:parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament .
    ?party
        a :Party ;
        :partyName ?partyName ;
        :count ?memberCount .
}
WHERE {
    SELECT ?parliament ?startDate ?endDate ?parliamentNumber ?party ?partyName ?nextParliament ?previousParliament (COUNT(?member) AS ?memberCount)
    WHERE {
        BIND(@parliamentid AS ?parliament)
        ?parliament 
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?startDate ;
            :parliamentPeriodNumber ?parliamentNumber .
        OPTIONAL { ?parliament :parliamentPeriodEndDate ?endDate . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
            ?seatIncumbency :incumbencyHasMember ?member ;
                            :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?member :partyMemberHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasParty ?party ;
        				     :partyMembershipStartDate ?partyMembershipStartDate .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }               
            ?party :partyName ?partyName .

            BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)
            
            BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
            BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
            FILTER (
        	    (?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	    (?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		    )
        }
    }
    GROUP BY ?parliament ?startDate ?endDate ?parliamentNumber ?party ?partyName ?nextParliament ?previousParliament
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}", Name = "ParliamentParty")]
        [HttpGet]
        public Graph parliament_party(string parliament_id, string party_id)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?parliament
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate ;
        :parliamentPeriodEndDate ?endDate ;
        :parliamentPeriodNumber ?parliamentNumber ;
        :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	:parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament .
    ?party
        a :Party ;
        :partyName ?partyName ;
        :count ?memberCount .
}
WHERE {
    SELECT ?parliament ?startDate ?endDate ?parliamentNumber ?party ?partyName ?nextParliament ?previousParliament (COUNT(?member) AS ?memberCount)
    WHERE {
        BIND(@parliamentid AS ?parliament)
        ?parliament 
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?startDate ;
            :parliamentPeriodNumber ?parliamentNumber .
        OPTIONAL { ?parliament :parliamentPeriodEndDate ?endDate . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
        OPTIONAL {
            BIND(@partyid AS ?party)
            ?party
                 a :Party ;
                 :partyName ?partyName .
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
            ?seatIncumbency :incumbencyHasMember ?member ;
                            :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?member :partyMemberHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasParty ?party ;
                             :partyMembershipStartDate ?partyMembershipStartDate .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . } 
              
            BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)
            
            BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
            BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
            FILTER (
                (?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
                (?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
            )
        }
    }
    GROUP BY ?parliament ?startDate ?endDate ?parliamentNumber ?party ?partyName ?nextParliament ?previousParliament
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}/members", Name = "ParliamentPartyMembers")]
        [HttpGet]
        public Graph parliament_party_members(string parliament_id, string party_id)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?seatIncumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?incStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .   
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?pmStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
     ?parliament 
         a :ParliamentPeriod ;
         :parliamentPeriodStartDate ?parliamentStartDate ;
         :parliamentPeriodEndDate ?parliamentEndDate ;
         :parliamentPeriodNumber ?parliamentNumber ;
         :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	 :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament ;
         :count ?memberCount .
    ?house
        a :House ;
        :houseName ?houseName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
             BIND(@parliamentid AS ?parliament)
             BIND(@partyid AS ?party)
	?party
         a :Party ;
         :partyName ?partyName .
    ?parliament 
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?parliamentStartDate ;
        :parliamentPeriodNumber ?parliamentNumber .
    OPTIONAL { ?parliament :parliamentPeriodEndDate ?parliamentEndDate . }
    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
    OPTIONAL {
        ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :incumbencyHasMember ?person ;
                        :incumbencyStartDate ?seatIncumbencyStartDate ;
                        :seatIncumbencyHasHouseSeat ?houseSeat .
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            
        ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup ;
                    :houseSeatHasHouse ?house .
        ?house :houseName ?houseName .
        ?constituencyGroup :constituencyGroupName ?constituencyName .

        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person :memberHasMemberImage ?image . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            
        ?person :partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership :partyMembershipHasParty ?party ;
        				 :partyMembershipStartDate ?partyMembershipStartDate .
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }  
             
		BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)
                
        BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
        BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
        FILTER (
        	(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		)
    }
   }
  }
  UNION {
	SELECT ?parliament (COUNT(DISTINCT(?person)) AS ?memberCount) WHERE {
             BIND(@parliamentid AS ?parliament)
             BIND(@partyid AS ?party)

        ?parliament a :ParliamentPeriod .
        ?party a :Party .
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
         	?seatIncumbency :incumbencyHasMember ?person ;
                            :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }

            ?person :partyMemberHasPartyMembership ?partyMembership .
        	?partyMembership :partyMembershipHasParty ?party ;
        				 	:partyMembershipStartDate ?partyMembershipStartDate .
        	OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . } 
              
            BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)
                
        	BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
        	BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
        	FILTER (
        		(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        		(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
			)
        }
      }
    GROUP BY ?parliament
  }  
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
             BIND(@parliamentid AS ?parliament)
             BIND(@partyid AS ?party)
        
            ?parliament a :ParliamentPeriod .
            ?party a :Party .
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
            ?seatIncumbency :incumbencyHasMember ?person ;
    					    :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?person :partyMemberHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasParty ?party ;
                             :partyMembershipStartDate ?partyMembershipStartDate .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }   
    
            BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)
            
            BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
            BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
            FILTER (
        	    (?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	    (?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		    )
            
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }         
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}/members/a_z_letters", Name = "ParliamentPartyMembersAToZLetters")]
        [HttpGet]
        public Graph parliament_party_members_a_to_z(string parliament_id, string party_id)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    _:x :value ?firstLetter .
}
WHERE {
    SELECT DISTINCT ?firstLetter WHERE {
             BIND(@parliamentid AS ?parliament)
             BIND(@partyid AS ?party)
        
        ?parliament a :ParliamentPeriod .
        ?party a :Party .
        ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :incumbencyHasMember ?person ;
    					:incumbencyStartDate ?seatIncumbencyStartDate .
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        ?person :partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership :partyMembershipHasParty ?party ;
                         :partyMembershipStartDate ?partyMembershipStartDate .
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }   
    
        BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)
        
        BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
        BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
        FILTER (
        	(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		)
            
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}/members/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ParliamentPartyMembersByInitial")]
        [HttpGet]
        public Graph parliament_party_members_by_initial(string parliament_id, string party_id, string initial)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?seatIncumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?incStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .   
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?pmStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
     ?parliament 
         a :ParliamentPeriod ;
         :parliamentPeriodStartDate ?parliamentStartDate ;
         :parliamentPeriodEndDate ?parliamentEndDate ;
         :parliamentPeriodNumber ?parliamentNumber ;
         :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	 :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament ;
         :count ?memberCount .
    ?house
        a :House ;
        :houseName ?houseName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
             BIND(@parliamentid AS ?parliament)
             BIND(@partyid AS ?party)
	?party
         a :Party ;
         :partyName ?partyName .
    ?parliament 
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?parliamentStartDate ;
        :parliamentPeriodNumber ?parliamentNumber .
    OPTIONAL { ?parliament :parliamentPeriodEndDate ?parliamentEndDate . }
    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
   	OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }        
    OPTIONAL {
        ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :incumbencyHasMember ?person ;
                        :incumbencyStartDate ?seatIncumbencyStartDate ;
                        :seatIncumbencyHasHouseSeat ?houseSeat .
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            
            ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup ;
                       :houseSeatHasHouse ?house .
            ?house :houseName ?houseName .
            ?constituencyGroup :constituencyGroupName ?constituencyName .

            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            
        ?person :partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership :partyMembershipHasParty ?party ;
        				 :partyMembershipStartDate ?partyMembershipStartDate .
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }  
             
        BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)

        BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
        BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
        FILTER (
        	(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		)    
        FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))        
      }   
     }
    }
  UNION {
	SELECT ?parliament (COUNT(DISTINCT(?person)) AS ?memberCount) WHERE {
             BIND(@parliamentid AS ?parliament)
             BIND(@partyid AS ?party)

        ?parliament a :ParliamentPeriod .
        ?party a :Party .
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
         	?seatIncumbency :incumbencyHasMember ?person ;
                            :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }

            ?person :partyMemberHasPartyMembership ?partyMembership .
        	?partyMembership :partyMembershipHasParty ?party ;
        				 	:partyMembershipStartDate ?partyMembershipStartDate .
        	OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
               
            BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)

        	BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
        	BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
        	FILTER (
        		(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        		(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
			)
        }
      }
    GROUP BY ?parliament
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
             BIND(@parliamentid AS ?parliament)
             BIND(@partyid AS ?party)
        
        ?parliament a :ParliamentPeriod.
        ?party a :Party.
        ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :incumbencyHasMember ?person ;
    					:incumbencyStartDate ?seatIncumbencyStartDate.
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        ?person :partyMemberHasPartyMembership ?partyMembership.
        ?partyMembership :partyMembershipHasParty ?party ;
                         :partyMembershipStartDate ?partyMembershipStartDate.
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }

        BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)  
            
        BIND(COALESCE(?pmEndDateTime, now()) AS ?pmEndDate)
        BIND(COALESCE(?incEndDateTime, now()) AS ?incEndDate)
        FILTER(
        	(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		)
            
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }         
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/houses/{houseid:regex(^\w{8}$)}/parties", Name = "ParliamentHouseParties")]
        [HttpGet]
        public Graph parliament_house_parties(string parliament_id, string house_id)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?parliament
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate ;
        :parliamentPeriodEndDate ?endDate ;
        :parliamentPeriodNumber ?parliamentNumber ;
        :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
   	    :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament .
    ?party
        a :Party ;
        :partyName ?partyName ;
        :count ?memberCount .
    ?house
        a :House ;
        :houseName ?houseName .
}
WHERE {
    SELECT ?parliament ?startDate ?endDate ?parliamentNumber ?nextParliament ?previousParliament ?party ?partyName ?house ?houseName (COUNT(?member) AS ?memberCount) WHERE {
        BIND(@parliamentid AS ?parliament)
    	BIND(@houseid AS ?house)
    	?house
        	a :House ;
         	:houseName ?houseName .
        ?parliament 
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?startDate ;
            :parliamentPeriodNumber ?parliamentNumber .
        OPTIONAL { ?parliament :parliamentPeriodEndDate ?endDate . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
   	    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
    	
    OPTIONAL {
        ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :incumbencyHasMember ?member ;
                        :incumbencyStartDate ?seatIncumbencyStartDate ;
                        :seatIncumbencyHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasHouse ?house .
        ?house :houseName ?houseName .
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        ?member :partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership :partyMembershipHasParty ?party ;
        				 :partyMembershipStartDate ?partyMembershipStartDate .
        ?party :partyName ?partyName .
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }

        BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)
            
        BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
        BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
        FILTER (
        	(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		)
    }
   }
   GROUP BY ?parliament ?startDate ?endDate ?parliamentNumber ?nextParliament ?previousParliament ?party ?partyName ?house ?houseName
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/houses/{houseid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}", Name = "ParliamentHouseParty")]
        [HttpGet]
        public Graph parliament_house_party(string parliament_id, string house_id, string party_id)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?parliament
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate ;
        :parliamentPeriodEndDate ?endDate ;
        :parliamentPeriodNumber ?parliamentNumber ;
         :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	 :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament .
    ?party
        a :Party ;
        :partyName ?partyName .
    ?house
        a :House ;
        :houseName ?houseName .
}
WHERE {
        BIND(@parliamentid AS ?parliament)
    	BIND(@houseid AS ?house)

    	?house
        	a :House ;
         	:houseName ?houseName .
        ?parliament 
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?startDate ;
            :parliamentPeriodNumber ?parliamentNumber .
        OPTIONAL { ?parliament :parliamentPeriodEndDate ?endDate . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
   	    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
    	
    OPTIONAL {
        BIND(@partyid AS ?party)

    	?party
        	a :Party ;
         	:partyName ?partyName .
        ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :incumbencyHasMember ?member ;
                        :incumbencyStartDate ?seatIncumbencyStartDate ;
                        :seatIncumbencyHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasHouse ?house .
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        ?member :partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership :partyMembershipHasParty ?party ;
        				 :partyMembershipStartDate ?partyMembershipStartDate .
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }

        BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)
        
        BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
        BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
        FILTER (
        	(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		)
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteSingle(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/houses/{houseid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}/members", Name = "ParliamentHousePartyMembers")]
        [HttpGet]
        public Graph parliament_house_party_members(string parliament_id, string house_id, string party_id)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?seatIncumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?incStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .   
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?pmStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
     ?parliament 
         a :ParliamentPeriod ;
         :parliamentPeriodStartDate ?parliamentStartDate ;
         :parliamentPeriodEndDate ?parliamentEndDate ;
         :parliamentPeriodNumber ?parliamentNumber ;
         :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	 :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament ;
         :count ?memberCount .
    ?house
        a :House ;
        :houseName ?houseName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@parliamentid AS ?parliament)
        BIND(@partyid AS ?party)
    	BIND(@houseid AS ?house)

    	?party
        	a :Party ;
         	:partyName ?partyName .
        ?parliament 
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?parliamentStartDate ;
            :parliamentPeriodNumber ?parliamentNumber .
    	?house
        	a :House ;
         	:houseName ?houseName .
        OPTIONAL { ?parliament :parliamentPeriodEndDate ?parliamentEndDate . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
   	    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
    	
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
            ?seatIncumbency :incumbencyHasMember ?person ;
                            :incumbencyStartDate ?seatIncumbencyStartDate ;
                            :seatIncumbencyHasHouseSeat ?houseSeat .
            ?houseSeat :houseSeatHasHouse ?house ;
                	    :houseSeatHasConstituencyGroup ?constituencyGroup .
            ?constituencyGroup :constituencyGroupName ?constituencyName .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?person :partyMemberHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasParty ?party ;
        				     :partyMembershipStartDate ?partyMembershipStartDate .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }

            BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)    
                
            BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
            BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
            FILTER (
        	    (?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	    (?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		    )
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        }
      }
    }
    UNION {
		SELECT ?parliament (COUNT(DISTINCT(?person)) AS ?memberCount) WHERE {
        BIND(@parliamentid AS ?parliament)
        BIND(@partyid AS ?party)
    	BIND(@houseid AS ?house)

        ?parliament a :ParliamentPeriod .
        ?house a :House .
        ?party a :Party .
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
         	?seatIncumbency :incumbencyHasMember ?person ;
                          	:seatIncumbencyHasHouseSeat ?houseSeat ;
                            :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?houseSeat :houseSeatHasHouse ?house .
            ?person :partyMemberHasPartyMembership ?partyMembership .
        	?partyMembership :partyMembershipHasParty ?party ;
        				 	:partyMembershipStartDate ?partyMembershipStartDate .
        	OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }  
             
            BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)

        	BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
        	BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
        	FILTER (
        		(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        		(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
			)
        }
      }
    GROUP BY ?parliament
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
        BIND(@parliamentid AS ?parliament)
        BIND(@partyid AS ?party)
    	BIND(@houseid AS ?house)
		
            ?party a :Party .
            ?house a :House .
            ?parliament a :ParliamentPeriod ;                
        			    :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
            ?seatIncumbency :incumbencyHasMember ?person ;
                            :seatIncumbencyHasHouseSeat ?houseSeat ;
            			    :incumbencyStartDate ?seatIncumbencyStartDate.
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?houseSeat :houseSeatHasHouse ?house .
            ?person :partyMemberHasPartyMembership ?partyMembership.
            ?partyMembership :partyMembershipHasParty ?party ;
                             :partyMembershipStartDate ?partyMembershipStartDate.
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }

            BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)
                
            BIND(COALESCE(?pmEndDateTime, now()) AS ?pmEndDate)
            BIND(COALESCE(?incEndDateTime, now()) AS ?incEndDate)
            FILTER(
        	    (?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	    (?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		    )
            
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/houses/{houseid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}/members/a_z_letters", Name = "ParliamentHousePartyMembersAToZLetters")]
        [HttpGet]
        public Graph parliament_house_party_members_a_to_z(string parliament_id, string house_id, string party_id)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
	_:x :value ?firstLetter .
}
WHERE {
   SELECT DISTINCT ?firstLetter WHERE {
        BIND(@parliamentid AS ?parliament)
        BIND(@partyid AS ?party)
    	BIND(@houseid AS ?house)
		
        ?party a :Party .
        ?house a :House .
        ?parliament a :ParliamentPeriod ;                
        			:parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :incumbencyHasMember ?person ;
                        :seatIncumbencyHasHouseSeat ?houseSeat ;
            			:incumbencyStartDate ?seatIncumbencyStartDate.
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        ?houseSeat :houseSeatHasHouse ?house .
        ?person :partyMemberHasPartyMembership ?partyMembership.
        ?partyMembership :partyMembershipHasParty ?party ;
                         :partyMembershipStartDate ?partyMembershipStartDate.
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }

        BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate) 
        
        BIND(COALESCE(?pmEndDateTime, now()) AS ?pmEndDate)
        BIND(COALESCE(?incEndDateTime, now()) AS ?incEndDate)
        FILTER(
        	(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		)
            
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
   }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{parliamentid:regex(^\w{8}$)}/houses/{houseid:regex(^\w{8}$)}/parties/{partyid:regex(^\w{8}$)}/members/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ParliamentHousePartyMembersByInitial")]
        [HttpGet]
        public Graph parliament_house_party_members_by_initial(string parliament_id, string house_id, string party_id, string initial)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?seatIncumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyStartDate ?incStartDate ;
        :incumbencyEndDate ?seatIncumbencyEndDate .   
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?pmStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
     ?parliament 
         a :ParliamentPeriod ;
         :parliamentPeriodStartDate ?parliamentStartDate ;
         :parliamentPeriodEndDate ?parliamentEndDate ;
         :parliamentPeriodNumber ?parliamentNumber ;
         :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	 :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament ;
         :count ?memberCount .
    ?house
        a :House ;
        :houseName ?houseName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@parliamentid AS ?parliament)
        BIND(@partyid AS ?party)
    	BIND(@houseid AS ?house)

    	?party
        	a :Party ;
         	:partyName ?partyName .
        ?parliament 
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?parliamentStartDate ;
            :parliamentPeriodNumber ?parliamentNumber .
    	?house
        	a :House ;
         	:houseName ?houseName .
        OPTIONAL { ?parliament :parliamentPeriodEndDate ?parliamentEndDate . }
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
   	    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
    	
    OPTIONAL {
        ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :incumbencyHasMember ?person ;
                        :incumbencyStartDate ?seatIncumbencyStartDate ;
                        :seatIncumbencyHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasHouse ?house ;
                	:houseSeatHasConstituencyGroup ?constituencyGroup .
        ?constituencyGroup :constituencyGroupName ?constituencyName .
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        ?person :partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership :partyMembershipHasParty ?party ;
        				 :partyMembershipStartDate ?partyMembershipStartDate .
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }

        BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)          
                
        BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
        BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
        FILTER (
        	(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		)
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person :memberHasMemberImage ?image . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

        FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))     
      }
    }
   }
   UNION {
		SELECT ?parliament (COUNT(DISTINCT(?person)) AS ?memberCount) WHERE {
        BIND(@parliamentid AS ?parliament)
        BIND(@partyid AS ?party)
    	BIND(@houseid AS ?house)

        ?parliament a :ParliamentPeriod .
        ?house a :House .
        ?party a :Party .
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
         	?seatIncumbency :incumbencyHasMember ?person ;
                          	:seatIncumbencyHasHouseSeat ?houseSeat ;
                            :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?houseSeat :houseSeatHasHouse ?house .
            ?person :partyMemberHasPartyMembership ?partyMembership .
        	?partyMembership :partyMembershipHasParty ?party ;
        				 	:partyMembershipStartDate ?partyMembershipStartDate .
        	OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }  
             
			BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)  
                
        	BIND(COALESCE(?pmEndDateTime,now()) AS ?pmEndDate)
        	BIND(COALESCE(?incEndDateTime,now()) AS ?incEndDate)
        	FILTER (
        		(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        		(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
			)
        }
      }
    GROUP BY ?parliament
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
        BIND(@parliamentid AS ?parliament)
        BIND(@partyid AS ?party)
    	BIND(@houseid AS ?house)

        ?party a :Party.
        ?house a :House.
        ?parliament a :ParliamentPeriod ;                
        			:parliamentPeriodHasSeatIncumbency ?seatIncumbency.
        ?seatIncumbency :incumbencyHasMember ?person ;
                        :seatIncumbencyHasHouseSeat ?houseSeat;
            			:incumbencyStartDate ?seatIncumbencyStartDate.
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        ?houseSeat :houseSeatHasHouse ?house.
        ?person :partyMemberHasPartyMembership ?partyMembership.
        ?partyMembership :partyMembershipHasParty ?party ;
                         :partyMembershipStartDate ?partyMembershipStartDate.
        OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }

		BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)         
            
        BIND(COALESCE(?pmEndDateTime, now()) AS ?pmEndDate)
        BIND(COALESCE(?incEndDateTime, now()) AS ?incEndDate)
        FILTER(
        	(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        	(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
		)
            
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/constituencies", Name = "ParliamentConstituencies")]
        [HttpGet]
        public Graph parliament_constituencies(string parliament_id)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?parliament
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate ;
        :parliamentPeriodEndDate ?endDate ;
        :parliamentPeriodNumber ?parliamentNumber ;
        :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	:parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament ;
        :count ?constituencyCount .
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyGroupName ;
        :constituencyGroupHasHouseSeat ?houseSeat .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyHasMember ?person ;
        :incumbencyStartDate ?incStartDate ;
        :incumbencyEndDate ?incEndDate .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?pmStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
_:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
    BIND(@parliamentid AS ?parliament)
        ?parliament 
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?startDate ;
            :parliamentPeriodNumber ?parliamentNumber .
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
   	    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
        OPTIONAL { ?parliament :parliamentPeriodEndDate ?endDate . }
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
            ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat ;
                            :incumbencyHasMember ?person ;
                            :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
            ?constituencyGroup :constituencyGroupName ?constituencyGroupName .
			
            ?person :partyMemberHasPartyMembership ?partyMembership.
        	?partyMembership :partyMembershipHasParty ?party ;
                         	:partyMembershipStartDate ?partyMembershipStartDate.
        	OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }

            BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)   
                
        	BIND(COALESCE(?pmEndDateTime, now()) AS ?pmEndDate)
        	BIND(COALESCE(?incEndDateTime, now()) AS ?incEndDate)
        	FILTER(
        		(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        		(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
			)   
            ?party :partyName ?partyName .
                
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        }
      }
    }
    UNION {
        SELECT ?parliament (COUNT(DISTINCT(?constituencyGroup)) AS ?constituencyCount) WHERE {
            BIND(@parliamentid AS ?parliament)
            
			?parliament a :ParliamentPeriod .
            OPTIONAL {
                ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
                ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
            }
        }
        GROUP BY ?parliament
    }
    UNION {
      SELECT DISTINCT ?firstLetter WHERE {
        BIND(@parliamentid AS ?parliament)

        ?parliament a :ParliamentPeriod ;
                    :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
        ?constituencyGroup :constituencyGroupName ?constituencyGroupName .

          BIND(ucase(SUBSTR(?constituencyGroupName, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/constituencies/a_z_letters", Name = "ParliamentConstituenciesAToZLetters")]
        [HttpGet]
        public Graph parliament_constituencies_a_to_z(string parliament_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
  _:x :value ?firstLetter .
}
WHERE {
    SELECT DISTINCT ?firstLetter WHERE {
        BIND(@parliamentid AS ?parliament)

        ?parliament a :ParliamentPeriod ;
                    :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
        ?constituencyGroup :constituencyGroupName ?constituencyGroupName .
        BIND(ucase(SUBSTR(?constituencyGroupName, 1, 1)) as ?firstLetter)
    } 
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/constituencies/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ParliamentConstituenciesByInitial")]
        [HttpGet]
        public Graph parliament_constituencies_by_initial(string parliament_id, string initial)
        {
            var queryString = @"
PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?parliament
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate ;
        :parliamentPeriodEndDate ?endDate ;
        :parliamentPeriodNumber ?parliamentNumber ;
        :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament ;
    	:parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament ;
        :count ?constituencyCount .
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyGroupName ;
        :constituencyGroupHasHouseSeat ?houseSeat .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyHasMember ?person ;
        :incumbencyStartDate ?incStartDate ;
        :incumbencyEndDate ?incEndDate .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipStartDate ?pmStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
    BIND(@parliamentid AS ?parliament)
        ?parliament 
            a :ParliamentPeriod ;
            :parliamentPeriodStartDate ?startDate ;
            :parliamentPeriodNumber ?parliamentNumber .
        OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyFollowingParliamentPeriod ?nextParliament . }
   	    OPTIONAL { ?parliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament . }
        OPTIONAL { ?parliament :parliamentPeriodEndDate ?endDate . }
        OPTIONAL {
            ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
            ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat ;
                            :incumbencyHasMember ?person ;
                            :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
            ?constituencyGroup :constituencyGroupName ?constituencyGroupName .
			
            ?person :partyMemberHasPartyMembership ?partyMembership.
        	?partyMembership :partyMembershipHasParty ?party ;
                         	:partyMembershipStartDate ?partyMembershipStartDate.
        	OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }

            BIND(xsd:dateTime(?partyMembershipEndDate) AS ?pmEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyEndDate) AS ?incEndDateTime)
        	BIND(xsd:dateTime(?seatIncumbencyStartDate) AS ?incStartDate)
        	BIND(xsd:dateTime(?partyMembershipStartDate) AS ?pmStartDate)   
                
        	BIND(COALESCE(?pmEndDateTime, now()) AS ?pmEndDate)
        	BIND(COALESCE(?incEndDateTime, now()) AS ?incEndDate)
        	FILTER(
        		(?pmStartDate <= ?incStartDate && ?pmEndDate > ?incStartDate) ||
        		(?pmStartDate >= ?incStartDate && ?pmStartDate < ?incEndDate)
			)   
            ?party :partyName ?partyName .
                
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            
                FILTER STRSTARTS(LCASE(?constituencyGroupName), LCASE(@initial))
        }
      }
    }
    UNION {
        SELECT ?parliament (COUNT(DISTINCT(?constituencyGroup)) AS ?constituencyCount) WHERE {
            BIND(@parliamentid AS ?parliament)
            
			?parliament a :ParliamentPeriod .
            OPTIONAL {
                ?parliament :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
                ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
            }
        }
        GROUP BY ?parliament
    }
    UNION {
      SELECT DISTINCT ?firstLetter WHERE {
        BIND(@parliamentid AS ?parliament)

        ?parliament a :ParliamentPeriod ;
                    :parliamentPeriodHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
        ?constituencyGroup :constituencyGroupName ?constituencyGroupName .

          BIND(ucase(SUBSTR(?constituencyGroupName, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }
    }
}
