namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {

        //[Route(@"{id:regex(^\w{8}$)}", Name = "ConstituencyByID")]
        [HttpGet]
        public Graph constituency_by_id(string constituency_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupEndDate ?endDate ;
        :constituencyGroupStartDate ?startDate ;
        :constituencyGroupName ?name ;
        :constituencyGroupOnsCode ?onsCode ;
        :constituencyGroupHasConstituencyArea ?constituencyArea .
    ?constituencyGroup :constituencyGroupHasHouseSeat ?houseSeat .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyHasMember ?member ;
        :incumbencyEndDate ?seatIncumbencyEndDate ;
        :incumbencyStartDate ?seatIncumbencyStartDate .
    ?member
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party .
    ?party
        a :Party ;
        :partyName ?partyName .
}
WHERE {
    BIND( @id AS ?constituencyGroup )
    ?constituencyGroup :constituencyGroupStartDate ?startDate .
    OPTIONAL { ?constituencyGroup :constituencyGroupEndDate ?endDate . }
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
    OPTIONAL { ?constituencyGroup :constituencyGroupOnsCode ?onsCode . }
    OPTIONAL {
        ?constituencyGroup :constituencyGroupHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency a :SeatIncumbency ;
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        OPTIONAL { ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate . }
        OPTIONAL {
            ?seatIncumbency :incumbencyHasMember ?member .
            OPTIONAL { ?member :personGivenName ?givenName . }
            OPTIONAL { ?member :personOtherName ?personOtherName . }
            OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            OPTIONAL { ?member :memberHasMemberImage ?image . }
            OPTIONAL {
                ?member :partyMemberHasPartyMembership ?partyMembership .
                FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
                OPTIONAL {
                    ?partyMembership :partyMembershipHasParty ?party .
                    OPTIONAL { ?party :partyName ?partyName . }
                }
            }
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, constituency_id));

            return BaseController.ExecuteSingle(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/map", Name = "ConstituencyMap")]
        [HttpGet]
        public Graph constituency_map(string constituency_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupEndDate ?endDate ;
        :constituencyGroupStartDate ?startDate ;
        :constituencyGroupName ?name ;
        :constituencyGroupOnsCode ?onsCode ;
        :constituencyGroupHasConstituencyArea ?constituencyArea .
    ?constituencyArea
        a :ConstituencyArea ;
        :constituencyAreaLatitude ?latitude ;
        :constituencyAreaLongitude ?longitude ;
        :constituencyAreaExtent ?polygon .
    ?constituencyGroup :constituencyGroupHasHouseSeat ?houseSeat .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyHasMember ?member ;
        :incumbencyEndDate ?seatIncumbencyEndDate ;
        :incumbencyStartDate ?seatIncumbencyStartDate .
    ?member
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party .
    ?party
        a :Party ;
        :partyName ?partyName .
}
WHERE {
    BIND( @id AS ?constituencyGroup )
    ?constituencyGroup a :ConstituencyGroup .
    OPTIONAL { ?constituencyGroup :constituencyGroupEndDate ?endDate . }
    OPTIONAL { ?constituencyGroup :constituencyGroupStartDate ?startDate . }
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
    OPTIONAL { ?constituencyGroup :constituencyGroupOnsCode ?onsCode . }
    OPTIONAL {
        ?constituencyGroup :constituencyGroupHasConstituencyArea ?constituencyArea .
        ?constituencyArea a :ConstituencyArea .
        OPTIONAL { ?constituencyArea :constituencyAreaLatitude ?latitude . }
        OPTIONAL { ?constituencyArea :constituencyAreaLongitude ?longitude . }
        OPTIONAL { ?constituencyArea :constituencyAreaExtent ?polygon . }
    }
    OPTIONAL {
        ?constituencyGroup :constituencyGroupHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency a :SeatIncumbency ;
        OPTIONAL { ?seatIncumbency :incumbencyHasMember ?member . }
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        OPTIONAL { ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate . }
        OPTIONAL { ?member :personGivenName ?givenName . }
        OPTIONAL { ?member :personFamilyName ?familyName . }
        OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        OPTIONAL { ?member :partyMemberHasPartyMembership ?partyMembership .}
        OPTIONAL { ?partyMembership :partyMembershipHasParty ?party . }
        OPTIONAL { ?party :partyName ?partyName . }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, constituency_id));

            return BaseController.ExecuteSingle(query);
        }

        //[Route(@"{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ConstituencyByInitial")]
        [HttpGet]
        public Graph constituency_by_initial(string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name ;
        :constituencyGroupEndDate ?endDate ;
        :constituencyGroupStartDate ?startDate ;
        :constituencyGroupHasHouseSeat ?seat .
    ?seat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyHasMember ?member ;
        :incumbencyHasEndDate ?seatIncumbencyEndDate .
    ?member
        a :Person;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party .
    ?party
        a :Party ;
        :partyName ?partyName .
    _:x :value ?firstLetter .

}
WHERE {
    { SELECT * WHERE {
        ?constituencyGroup a :ConstituencyGroup .
        OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
        OPTIONAL { ?constituencyGroup :constituencyGroupEndDate ?endDate . }
        OPTIONAL { ?constituencyGroup :constituencyGroupStartDate ?startDate . }
        OPTIONAL {
            ?constituencyGroup :constituencyGroupHasHouseSeat ?seat .
            ?seat :houseSeatHasSeatIncumbency ?seatIncumbency .
            OPTIONAL { ?seatIncumbency :incumbencyHasEndDate ?seatIncumbencyEndDate . }
            FILTER NOT EXISTS {?seatIncumbency a :PastIncumbency . }
            ?seatIncumbency :incumbencyHasMember ?member .
            OPTIONAL { ?member :personGivenName ?givenName . }
            OPTIONAL { ?member :personFamilyName ?familyName . }
            OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?member :partyMemberHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasParty ?party .
            ?party :partyName ?partyName .
        }
        FILTER STRSTARTS(LCASE(?name), LCASE(@initial))
       }
    }
    UNION {
		SELECT DISTINCT ?firstLetter WHERE {
            ?s a :ConstituencyGroup ;
          		:constituencyGroupName ?constituencyName.

              BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
        }
	}
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        //[Route("current", Name = "ConstituencyCurrent")]
        [HttpGet]
        public Graph constituency_current()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name ;
        :constituencyGroupHasHouseSeat ?seat .
    ?seat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyHasMember ?member .
    ?member
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        :partyMemberHasPartyMembership ?partyMembership .
     ?partyMembership
         a :PartyMembership ;
         :partyMembershipHasParty ?party .
     ?party
         a :Party ;
         :partyName ?partyName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        ?constituencyGroup a :ConstituencyGroup .
        FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
        OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
        OPTIONAL {
            ?constituencyGroup :constituencyGroupHasHouseSeat ?seat .
            ?seat :houseSeatHasSeatIncumbency ?seatIncumbency .
            FILTER NOT EXISTS { ?seatIncumbency a :PastIncumbency . }
            ?seatIncumbency :incumbencyHasMember ?member .
            OPTIONAL { ?member :personGivenName ?givenName . }
            OPTIONAL { ?member :personFamilyName ?familyName . }
            OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?member :partyMemberHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasParty ?party .
            ?party :partyName ?partyName .
        }
      }
    }
    UNION {
          SELECT DISTINCT ?firstLetter WHERE {
	        ?s a :ConstituencyGroup .
          	FILTER NOT EXISTS { ?s a :PastConstituencyGroup . }
          	?s :constituencyGroupName ?constituencyName .

          	BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);


            return BaseController.ExecuteList(query);
        }

        //[Route(@"lookup/{source:regex(^\w+$)}/{id}", Name = "ConstituencyLookup")]
        [HttpGet]
        public Graph constituency_lookup(string property, string value) => base.LookupInternal("ConstituencyGroup", property, value);

        //[Route(@"partial/{letters:regex(^\p{L}+$):minlength(2)}", Name = "ConstituencyByLetters", Order = 999)]
        [HttpGet]
        public Graph constituency_by_substring(string substring)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName ;
        :constituencyGroupEndDate ?endDate ;
        :constituencyGroupHasHouseSeat ?seat .
    ?seat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyHasMember ?member .
    ?member
        a :Person;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party .
    ?party
        a :Party ;
        :partyName ?partyName .
    _:x
        :value ?firstLetter .
      }
    WHERE {
    	{ SELECT * WHERE {
            ?constituencyGroup a :ConstituencyGroup ;
                                :constituencyGroupName ?constituencyName .
            OPTIONAL { ?constituencyGroup :constituencyGroupEndDate ?endDate . }
            OPTIONAL {
                ?constituencyGroup :constituencyGroupHasHouseSeat ?seat .
                ?seat :houseSeatHasSeatIncumbency ?seatIncumbency .
                FILTER NOT EXISTS { ?seatIncumbency a :PastIncumbency . }
                ?seatIncumbency :incumbencyHasMember ?member .
                OPTIONAL { ?member :personGivenName ?givenName . }
                OPTIONAL { ?member :personFamilyName ?familyName . }
                OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
                ?member :partyMemberHasPartyMembership ?partyMembership .
                FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
                ?partyMembership :partyMembershipHasParty ?party .
                ?party :partyName ?partyName .
           }
           FILTER CONTAINS(LCASE(?constituencyName), LCASE(@substring))
        }
    }
    UNION {
		    SELECT DISTINCT ?firstLetter WHERE {

                ?s a :ConstituencyGroup ;
                    :constituencyGroupName ?constituencyName.

                BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
            }
	    }
   }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("substring", substring);

            return BaseController.ExecuteList(query);
        }

        //[Route("a_z_letters", Name = "ConstituencyAToZ")]
        [HttpGet]
        public Graph constituency_a_to_z()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    [ :value ?firstLetter ]
}
WHERE {
    SELECT DISTINCT ?firstLetter
    WHERE {
        ?s
            a :ConstituencyGroup ;
            :constituencyGroupName ?constituencyName .
        BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }

        //[Route(@"current/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ConstituencyCurrentByInitial")]
        [HttpGet]
        public Graph constituency_current_by_initial(string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name ;
        :constituencyGroupHasHouseSeat ?seat .
    ?seat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyHasMember ?member .
    ?member
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        :partyMemberHasPartyMembership ?partyMembership .
     ?partyMembership
         a :PartyMembership ;
         :partyMembershipHasParty ?party .
     ?party
         a :Party ;
         :partyName ?partyName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        ?constituencyGroup a :ConstituencyGroup .
        FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
        OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
        OPTIONAL {
            ?constituencyGroup :constituencyGroupHasHouseSeat ?seat .
            ?seat :houseSeatHasSeatIncumbency ?seatIncumbency .
            FILTER NOT EXISTS { ?seatIncumbency a :PastIncumbency . }
            ?seatIncumbency :incumbencyHasMember ?member .
            OPTIONAL { ?member :personGivenName ?givenName . }
            OPTIONAL { ?member :personFamilyName ?familyName . }
            OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?member :partyMemberHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasParty ?party .
            ?party :partyName ?partyName .
        }
        FILTER STRSTARTS(LCASE(?name), LCASE(@initial))
      }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
          ?s a :ConstituencyGroup.
          FILTER NOT EXISTS { ?s a :PastConstituencyGroup. }
          ?s :constituencyGroupName ?constituencyName.

           BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        //[Route("current/a_z_letters", Name = "ConstituencyCurrentAToZ")]
        [HttpGet]
        public Graph constituency_current_a_to_z()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    [ :value ?firstLetter ]
}
WHERE {
    SELECT DISTINCT ?firstLetter
    WHERE {
        ?s a :ConstituencyGroup .
        FILTER NOT EXISTS { ?s a :PastConstituencyGroup . }
        ?s :constituencyGroupName ?constituencyName .
        BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }

        //[Route("", Name = "ConstituencyIndex")]
        [HttpGet]
        public Graph constituency_index()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name ;
        :constituencyGroupEndDate ?endDate ;
        :constituencyGroupStartDate ?startDate ;
        :constituencyGroupHasHouseSeat ?seat .
    ?seat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyHasMember ?member .
    ?member
        a :Person;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party .
    ?party
        a :Party ;
        :partyName ?partyName .
    _:x
        :value ?firstLetter .
      }
    WHERE {
    	{ SELECT * WHERE {
            ?constituencyGroup a :ConstituencyGroup ;
                                :constituencyGroupName ?name .
            OPTIONAL { ?constituencyGroup :constituencyGroupEndDate ?endDate . }
            OPTIONAL { ?constituencyGroup :constituencyGroupStartDate ?startDate . }
            OPTIONAL {
                ?constituencyGroup :constituencyGroupHasHouseSeat ?seat .
                ?seat :houseSeatHasSeatIncumbency ?seatIncumbency .
                FILTER NOT EXISTS {?seatIncumbency a :PastIncumbency . }
                ?seatIncumbency :incumbencyHasMember ?member .
                OPTIONAL { ?member :personGivenName ?givenName . }
                OPTIONAL { ?member :personFamilyName ?familyName . }
                OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
                ?member :partyMemberHasPartyMembership ?partyMembership .
                FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
                ?partyMembership :partyMembershipHasParty ?party .
                ?party :partyName ?partyName .
           }
        }
    }
    UNION {
		    SELECT DISTINCT ?firstLetter WHERE {
	            ?s a :ConstituencyGroup ;
                    :constituencyGroupName ?constituencyName .

                BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
            }
	    }
   }
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members", Name = "ConstituencyMembers")]
        [HttpGet]
        public Graph constituency_members(string constituency_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name ;
        :constituencyGroupHasHouseSeat ?houseSeat ;
        :constituencyGroupStartDate ?constituencyGroupStartDate ;
        :constituencyGroupEndDate ?constituencyGroupEndDate .
    ?houseSeat a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency a :SeatIncumbency ;
        :incumbencyHasMember ?member ;
        :incumbencyEndDate ?seatIncumbencyEndDate ;
        :incumbencyStartDate ?seatIncumbencyStartDate .
    ?member a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
}
WHERE {
    BIND( @constituencyid AS ?constituencyGroup )
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupHasHouseSeat ?houseSeat .
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
    OPTIONAL { ?constituencyGroup :constituencyGroupEndDate ?constituencyGroupEndDate . }
    OPTIONAL { ?constituencyGroup :constituencyGroupStartDate ?constituencyGroupStartDate . }
    OPTIONAL {
        ?houseSeat :houseSeatHasSeatIncumbency ?seatIncumbency .
        OPTIONAL {
            ?seatIncumbency :incumbencyHasMember ?member .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            OPTIONAL { ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate . }
            OPTIONAL { ?member :personGivenName ?givenName . }
            OPTIONAL { ?member :personFamilyName ?familyName . }
            OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("constituencyid", new Uri(BaseController.instance, constituency_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members/current", Name = "ConstituencyCurrentMember")]
        [HttpGet]
        public Graph constituency_current_member(string constituency_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name ;
        :constituencyGroupStartDate ?constituencyGroupStartDate ;
        :constituencyGroupEndDate ?constituencyGroupEndDate ;
        :constituencyGroupHasHouseSeat ?houseSeat .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyHasMember ?member ;
        :incumbencyEndDate ?seatIncumbencyEndDate ;
        :incumbencyStartDate ?seatIncumbencyStartDate .
    ?member
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
}
WHERE {
    BIND(@constituencyid AS ?constituencyGroup )
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupHasHouseSeat ?houseSeat .
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
    OPTIONAL { ?constituencyGroup :constituencyGroupStartDate ?constituencyGroupStartDate . }
    OPTIONAL { ?constituencyGroup :constituencyGroupEndDate ?constituencyGroupEndDate . }
    OPTIONAL {
        ?houseSeat :houseSeatHasSeatIncumbency ?seatIncumbency .
        FILTER NOT EXISTS { ?seatIncumbency a :PastIncumbency . }
        OPTIONAL {
            ?seatIncumbency :incumbencyHasMember ?member .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            OPTIONAL { ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate . }
            OPTIONAL { ?member :personGivenName ?givenName . }
            OPTIONAL { ?member :personFamilyName ?familyName . }
            OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("constituencyid", new Uri(BaseController.instance, constituency_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph find_your_constituency()
        {
            var queryString = @"
            PREFIX spatial: <http://data.ordnancesurvey.co.uk/ontology/spatialrelations/>
            PREFIX skos: <http://www.w3.org/2004/02/skos/core#>
            PREFIX admingeo: <http://data.ordnancesurvey.co.uk/ontology/admingeo/>
            PREFIX : <http://id.ukpds.org/schema/>
            CONSTRUCT {
              ?region skos:prefLabel ?name  .
              ?region a admingeo:EuropeanRegion .
              ?region admingeo:gssCode ?gssCode .
              ?region :count ?count .
            }
            WHERE {
              {
                SELECT ?region (COUNT(?westminsterConstituency) AS ?count) WHERE
                {
                    ?region a admingeo:EuropeanRegion .
                    ?region admingeo:gssCode ?gssCode .
                    ?region admingeo:westminsterConstituency ?westminsterConstituency .
                } GROUP BY ?region
              }
              UNION
              {
                SELECT * WHERE {
                    ?region skos:prefLabel ?name  .
                    ?region a admingeo:EuropeanRegion .
                    ?region admingeo:gssCode ?gssCode .
                }
              }
            }
            ";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }

        // TODO: Why is this singular? - still not completely solved here
        //[Route(@"{id:regex(^\w{8}$)}/contact_point", Name = "ConstituencyContactPoint")]
        [HttpGet]
        public Graph constituency_contact_point(string constituency_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupHasHouseSeat ?houseSeat ;
        :constituencyGroupName ?name .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?incumbency .
    ?incumbency
        a :SeatIncumbency ;
        :incumbencyHasContactPoint ?contactPoint .
    ?contactPoint
        a :ContactPoint ;
        :email ?email ;
        :phoneNumber ?phoneNumber ;
        :faxNumber ?faxNumber ;
        :contactForm ?contactForm ;
        :contactPointHasPostalAddress ?postalAddress .
    ?postalAddress
        a :PostalAddress ;
        :postCode ?postCode ;
        :addressLine1 ?addressLine1 ;
        :addressLine2 ?addressLine2 ;
        :addressLine3 ?addressLine3 ;
        :addressLine4 ?addressLine4 ;
        :addressLine5 ?addressLine5 .
}
WHERE {
    BIND(@constituencyid AS ?constituencyGroup )
    ?constituencyGroup a :ConstituencyGroup .
    OPTIONAL {
        ?constituencyGroup :constituencyGroupHasHouseSeat ?houseSeat .
        OPTIONAL {
            ?houseSeat :houseSeatHasSeatIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL {
                ?incumbency :incumbencyHasContactPoint ?contactPoint .
                OPTIONAL{ ?contactPoint :email ?email . }
                OPTIONAL{ ?contactPoint :phoneNumber ?phoneNumber . }
                OPTIONAL{ ?contactPoint :faxNumber ?faxNumber . }
                OPTIONAL{ ?contactPoint :contactForm ?contactForm . }
                OPTIONAL{
        			?contactPoint :contactPointHasPostalAddress ?postalAddress .
                    OPTIONAL{ ?postalAddress :postCode ?postCode . }
                    OPTIONAL{ ?postalAddress :addressLine1 ?addressLine1 . }
                    OPTIONAL{ ?postalAddress :addressLine2 ?addressLine2 . }
                    OPTIONAL{ ?postalAddress :addressLine3 ?addressLine3 . }
                    OPTIONAL{ ?postalAddress :addressLine4 ?addressLine4 . }
                    OPTIONAL{ ?postalAddress :addressLine5 ?addressLine5 . }
            	}
        	}
    	}
	}
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("constituencyid", new Uri(BaseController.instance, constituency_id));

            return BaseController.ExecuteSingle(query);
        }

        //[Route(@"postcode_lookup/{postcode}", Name = "ConstituencyLookupByPostcode")]
        [HttpGet]
        public Graph constituency_lookup_by_postcode(string postcode)
        {
            var externalQueryString = @"
PREFIX geo: <http://www.w3.org/2003/01/geo/wgs84_pos#>

CONSTRUCT {
    ?postcode
        geo:long ?long ;
        geo:lat ?lat .
}
WHERE {
    BIND(@postcode as ?postcode)
    ?postcode geo:long ?long ;
        geo:lat ?lat .
}
";
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
PREFIX geo: <http://www.opengis.net/ont/geosparql#>
PREFIX geof: <http://www.opengis.net/def/function/geosparql/>
construct {
    ?constituencyGroup
        a parl:ConstituencyGroup ;
        parl:constituencyGroupName ?constituencyGroupName ;
        parl:constituencyGroupHasHouseSeat ?houseSeat .
    ?houseSeat
        a parl:HouseSeat ;
        parl:houseSeatHasConstituencyGroup ?constituencyGroup .
    ?seatIncumbency
        a parl:SeatIncumbency ;
        parl:incumbencyStartDate ?incStartDate ;
        parl:incumbencyEndDate ?incEndDate ;
        parl:seatIncumbencyHasHouseSeat ?houseSeat .
    ?member
        a parl:Person ;
        parl:personGivenName ?givenName ;
        parl:personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        parl:memberHasIncumbency ?seatIncumbency ;
        parl:partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership
        a parl:PartyMembership ;
        parl:partyMembershipHasParty ?party ;
        parl:partyMembershipStartDate ?pmStartDate ;
        parl:partyMembershipEndDate ?pmEndDate .
    ?party
        a parl:Party ;
        parl:partyName ?partyName .
}
where {
    ?constituencyArea a parl:ConstituencyArea;
        parl:constituencyAreaExtent ?constituencyAreaExtent;
        parl:constituencyAreaHasConstituencyGroup ?constituencyGroup.
    ?constituencyGroup parl:constituencyGroupName ?constituencyGroupName.
    bind(strdt(concat(""Point("",@longitude,"" "",@latitude,"")""),geo:wktLiteral) as ?point)
    filter(geof:sfWithin(?point,?constituencyAreaExtent))

    optional {
        ?constituencyGroup parl:constituencyGroupHasHouseSeat ?houseSeat .
        ?houseSeat parl:houseSeatHasSeatIncumbency ?seatIncumbency .
        filter not exists { ?seatIncumbency a parl:PastIncumbency . }
        ?seatIncumbency parl:incumbencyStartDate ?incStartDate .
        optional { ?seatIncumbency parl:incumbencyEndDate ?incEndDate . }
        optional { ?seatIncumbency parl:incumbencyHasMember ?member . }
        optional { ?member parl:personGivenName ?givenName . }
        optional { ?member parl:personFamilyName ?familyName . }
        optional { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .

        optional {
            ?member parl:partyMemberHasPartyMembership ?partyMembership .
            filter not exists { ?partyMembership a parl:PastPartyMembership . }
            ?partyMembership parl:partyMembershipHasParty ?party ;
                             parl:partyMembershipStartDate ?pmStartDate .
            optional { ?partyMembership parl:partyMembershipEndDate ?pmEndDate . }
            ?party parl:partyName ?partyName .
        }
   }
}
";

            postcode = postcode.ToUpperInvariant().Replace(" ", string.Empty);

            GetCoordinates(postcode, externalQueryString, out string latitude, out string longitude);

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("longitude", longitude);
            query.SetLiteral("latitude", latitude);

            return BaseController.ExecuteSingle(query);
        }

        private static void GetCoordinates(string postcode, string externalQueryString, out string latitude, out string longitude)
        {
            if (postcode.StartsWith("BT"))
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        var json = client.GetStringAsync($"http://api.postcodes.io/postcodes/{postcode}").Result;

                        dynamic postcodesioJson = JsonConvert.DeserializeObject(json);

                        latitude = postcodesioJson.result.latitude ;
                        longitude = postcodesioJson.result.longitude ;
                    }
                    catch (AggregateException e) when (e.InnerException is HttpRequestException)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                }
            }
            else
            {
                var externalQuery = new SparqlParameterizedString(externalQueryString);
                externalQuery.SetUri("postcode", new Uri(new Uri("http://data.ordnancesurvey.co.uk/id/postcodeunit/"), postcode));
                var externalResults = BaseController.ExecuteList(externalQuery, "http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql");

                if (externalResults.Triples.Any())
                {
                    var longitudeObject = (LiteralNode)externalResults.GetTriplesWithPredicate(new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#long")).SingleOrDefault().Object;
                    var latitudeObject = (LiteralNode)externalResults.GetTriplesWithPredicate(new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#lat")).SingleOrDefault().Object;

                    longitude = longitudeObject.Value;
                    latitude = latitudeObject.Value;
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
            }
        }
    }
}
