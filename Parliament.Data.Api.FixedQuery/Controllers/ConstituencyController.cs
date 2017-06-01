namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("constituencies")]
    public class ConstituencyController : BaseController
    {
        [Route(@"{id:regex(^\w{8}$)}", Name = "ConstituencyByID")]
        [HttpGet]
        public Graph ById(string id)
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

            query.SetUri("id", new Uri(BaseController.instance, id));

            return BaseController.ExecuteSingle(query);
        }

        [Route(@"{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ConstituencyByInitial")]
        [HttpGet]
        public Graph ByInitial(string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT{
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name ;
        :constituencyGroupEndDate ?endDate ;
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

        [Route("current", Name = "ConstituencyCurrent")]
        [HttpGet]
        public Graph Current()
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

        [Route(@"lookup/{source:regex(^\w+$)}/{id}", Name = "ConstituencyLookup")]
        [HttpGet]
        public Graph Lookup(string source, string id) => base.LookupInternal("ConstituencyGroup", source, id);

        [Route(@"partial/{letters:regex(^\p{L}+$):minlength(2)}", Name = "ConstituencyByLetters", Order = 999)]
        [HttpGet]
        public Graph ByLetters(string letters)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituency
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName ;
        :constituencyGroupEndDate ?endDate .
}
WHERE {
    ?constituency a :ConstituencyGroup .
    ?constituency :constituencyGroupName ?constituencyName .
    OPTIONAL { ?constituency :constituencyGroupEndDate ?endDate . }
    FILTER CONTAINS(LCASE(?constituencyName), LCASE(@letters))
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letters", letters);

            return BaseController.ExecuteList(query);
        }

        [Route("a_z_letters", Name = "ConstituencyAToZ")]
        [HttpGet]
        public Graph AToZLetters()
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

        [Route(@"current/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ConstituencyCurrentByInitial")]
        [HttpGet]
        public Graph CurrentByLetters(string initial)
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

        [Route("current/a_z_letters", Name = "ConstituencyCurrentAToZ")]
        [HttpGet]
        public Graph CurrentAToZLetters()
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

        [Route("", Name = "ConstituencyIndex")]
        [HttpGet]
        public Graph Index()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name ;
        :constituencyGroupEndDate ?endDate ;
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
        :partyMembershipHasParty ?party ;
        :partyMembershipEndDate ?partyMembershipEndDate .
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
                FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
                OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
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

        [Route(@"{id:regex(^\w{8}$)}/members", Name = "ConstituencyMembers")]
        [HttpGet]
        public Graph Members(string id)
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

            query.SetUri("constituencyid", new Uri(BaseController.instance, id));

            return BaseController.ExecuteList(query);
        }

        [Route(@"{id:regex(^\w{8}$)}/members/current", Name = "ConstituencyCurrentMember")]
        [HttpGet]
        public Graph CurrentMembers(string id)
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

            query.SetUri("constituencyid", new Uri(BaseController.instance, id));

            return BaseController.ExecuteList(query);
        }

        // TODO: Why is this singular? - still not completely solved here
        [Route(@"{id:regex(^\w{8}$)}/contact_point", Name = "ConstituencyContactPoint")]
        [HttpGet]
        public Graph ContactPoint(string id)
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

            query.SetUri("constituencyid", new Uri(BaseController.instance, id));

            return BaseController.ExecuteSingle(query);
        }

        [Route(@"postcode_lookup/{postcode}", Name = "ConstituencyLookupByPostcode")]
        [HttpGet]
        public Graph LookupByPostcode(string postcode)
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
        parl:houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a parl:SeatIncumbency ;
        parl:incumbencyHasMember ?member .
    ?member
        a parl:Person ;
        parl:personGivenName ?givenName ;
        parl:personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        parl:partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership
        a parl:PartyMembership ;
        parl:partyMembershipHasParty ?party .
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
        optional { ?seatIncumbency parl:incumbencyHasMember ?member . }
        optional { ?member parl:personGivenName ?givenName . }
        optional { ?member parl:personFamilyName ?familyName . }
        optional { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        
        optional {
            ?member parl:partyMemberHasPartyMembership ?partyMembership .
            filter not exists { ?partyMembership a parl:PastPartyMembership . }
            ?partyMembership parl:partyMembershipHasParty ?party .
            ?party parl:partyName ?partyName .
        }
   }
}
";
            var externalQuery = new SparqlParameterizedString(externalQueryString);
            var formattedPostcode = postcode.ToUpperInvariant().Replace(" ", string.Empty);
            externalQuery.SetUri("postcode", new Uri(new Uri("http://data.ordnancesurvey.co.uk/id/postcodeunit/"), formattedPostcode));
            var externalResults = BaseController.ExecuteSingle(externalQuery, "http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql");

            var query = new SparqlParameterizedString(queryString);
            var longitude = (LiteralNode)externalResults.GetTriplesWithPredicate(new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#long")).SingleOrDefault().Object;
            var latitude = (LiteralNode)externalResults.GetTriplesWithPredicate(new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#lat")).SingleOrDefault().Object;
            query.SetLiteral("longitude", longitude.Value);
            query.SetLiteral("latitude", latitude.Value);

            return BaseController.ExecuteSingle(query);
        }
    }
}
