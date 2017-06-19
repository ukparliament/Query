namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("people")]
    public class PersonController : BaseController
    {
        [Route("", Name = "PersonIndex")]
        [HttpGet]
        public Graph Index()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
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
        ?person 
            a :Person ;
            :memberHasIncumbency ?incumbency .
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        { 
            ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency)
            OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        }
        UNION {
            ?incumbency a :SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency)
            ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                ?constituencyGroup :constituencyGroupName ?constituencyName .
                FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . } 
            }
        }
        OPTIONAL {
            ?person :partyMemberHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasParty ?party .
            ?party :partyName ?partyName .
        }
      }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
	        ?person a :Person ;
                    <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
}
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [Route(@"{id:regex(^\w{8}$)}", Name = "PersonById")]
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
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/D79B0BAC513C4A9A87C9D5AFF1FC632F> ?fullTitle ;
        :partyMemberHasPartyMembership ?partyMembership ;
        :memberHasIncumbency ?incumbency .
    ?contactPoint 
        a :ContactPoint ;
        :email ?email ;
        :phoneNumber ?phoneNumber ;
        :contactPointHasPostalAddress ?postalAddress .
    ?postalAddress 
        a :PostalAddress ;
        :addressLine1 ?addressLine1 ;
        :addressLine2 ?addressLine2 ;
        :addressLine3 ?addressLine3 ;
        :addressLine4 ?addressLine4 ;
        :addressLine5 ?addressLine5 ;
        :faxNumber ?faxNumber ;
        :postCode ?postCode .
    ?constituency 
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
    ?seatIncumbency 
        a :SeatIncumbency ;
        :incumbencyEndDate ?seatIncumbencyEndDate ;
        :incumbencyStartDate ?seatIncumbencyStartDate ;
        :seatIncumbencyHasHouseSeat ?seat ;
        :incumbencyHasContactPoint ?contactPoint .
    ?houseIncumbency 
        a :HouseIncumbency ;
        :incumbencyEndDate ?houseIncumbencyEndDate ;
        :incumbencyStartDate ?houseIncumbencyStartDate ;
        :houseIncumbencyHasHouse ?house1 ;
        :incumbencyHasContactPoint ?contactPoint .
    ?seat 
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituency ;
        :houseSeatHasHouse ?house2 .
    ?party 
        a :Party ;
        :partyName ?partyName .
    ?partyMembership 
        a :PartyMembership ;
        :partyMembershipStartDate ?partyMembershipStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate ;
        :partyMembershipHasParty ?party .
    ?house1 
        a :House ;
        :houseName ?houseName1 .
    ?house2 
        a :House ;
        :houseName ?houseName2 .
}
WHERE {
    BIND(@id AS ?person)
    ?person a :Person .
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personOtherNames ?otherName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
    OPTIONAL { ?person <http://example.com/D79B0BAC513C4A9A87C9D5AFF1FC632F> ?fullTitle } .
    OPTIONAL {
        ?person :memberHasIncumbency ?incumbency .
        OPTIONAL {
            ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency)
            ?houseIncumbency :houseIncumbencyHasHouse ?house1 .
            ?house1 :houseName ?houseName1 .
            ?houseIncumbency :incumbencyStartDate ?houseIncumbencyStartDate .
            OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        }
        OPTIONAL {
            ?incumbency a :SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency)
            ?seatIncumbency :seatIncumbencyHasHouseSeat ?seat .
            ?seat :houseSeatHasConstituencyGroup ?constituency .
            ?seat :houseSeatHasHouse ?house2 .
            ?house2 :houseName ?houseName2 .
            ?constituency :constituencyGroupName ?constituencyName .
            ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        }
        OPTIONAL {
            ?incumbency :incumbencyHasContactPoint ?contactPoint .
            OPTIONAL { ?contactPoint :phoneNumber ?phoneNumber . }
            OPTIONAL { ?contactPoint :email ?email . }
            OPTIONAL {
                ?contactPoint :contactPointHasPostalAddress ?postalAddress .
                OPTIONAL { ?postalAddress :addressLine1 ?addressLine1 . }
                OPTIONAL { ?postalAddress :addressLine2 ?addressLine2 . }
                OPTIONAL { ?postalAddress :addressLine3 ?addressLine3 . }
                OPTIONAL { ?postalAddress :addressLine4 ?addressLine4 . }
                OPTIONAL { ?postalAddress :addressLine5 ?addressLine5 . }
                OPTIONAL { ?postalAddress :faxNumber ?faxNumber . }
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

            return BaseController.ExecuteSingle(query);
        }

        [Route(@"{initial:regex(^\p{L}+$):maxlength(1)}", Name = "PersonByInitial")]
        [HttpGet]
        public Graph ByInitial(string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
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
        ?person 
            a :Person ;
            :memberHasIncumbency ?incumbency .
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        { 
            ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency)
            OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        }
        UNION {
            ?incumbency a :SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency)
            ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                ?constituencyGroup :constituencyGroupName ?constituencyName . 
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
            }
        }
        OPTIONAL {
            ?person :partyMemberHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasParty ?party .
            ?party :partyName ?partyName .
        }
        FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
      }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {

            ?person a :Person ;
          			<http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);

        }

        [Route(@"lookup/{source:regex(^\w+$)}/{id}", Name = "PersonLookup")]
        [HttpGet]
        public Graph Lookup(string source, string id) => base.LookupInternal("Person", source, id);

        [Route("members", Name = "MemberIndex")]
        [HttpGet]
        public Graph Member()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
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
        ?person 
            a :Person ;
            :memberHasIncumbency ?incumbency .
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        { 
            ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency)
            OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        }
        UNION {
            ?incumbency a :SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency)
            ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                ?constituencyGroup :constituencyGroupName ?constituencyName . 
                FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
            }
        }
        OPTIONAL {
            ?person :partyMemberHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasParty ?party .
            ?party :partyName ?partyName .
        }
      }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
	        ?incumbency a :Incumbency ;
                        :incumbencyHasMember ?person .
          ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
}
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [Route(@"partial/{letters:regex(^\p{L}+$):minlength(2)}", Name = "PersonByLetters", Order = 999)]
        [HttpGet]
        public Graph ByLetters(string letters)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
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
        ?person 
            a :Person ;
            :memberHasIncumbency ?incumbency .
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        { 
            ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency)
            OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        }
        UNION {
            ?incumbency a :SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency)
            ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
            OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                ?constituencyGroup :constituencyGroupName ?constituencyName .
                FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . } 
            }
        }
        OPTIONAL {
            ?person :partyMemberHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasParty ?party .
            ?party :partyName ?partyName .
        }
        FILTER (REGEX(STR(?displayAs), @letters, 'i'))

      }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {

            ?person a :Person ;
                    <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letters", letters);
            return BaseController.ExecuteList(query);
        }

        [Route("a_z_letters", Name = "PersonAToZ")]
        [HttpGet]
        public Graph AToZLetters()
        {
            var queryString = @"
PREFIX :<http://id.ukpds.org/schema/>
CONSTRUCT {
    [ :value ?firstLetter ]
}
WHERE {
    SELECT DISTINCT ?firstLetter WHERE {
        ?s a :Person .
        ?s <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }

        [Route(@"{id:regex(^\w{8}$)}/constituencies", Name = "PersonConstituencies")]
        [HttpGet]
        public Graph Constituencies(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person 
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
    ?constituency
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName ;
        :constituencyGroupStartDate ?constituencyStartDate ;
        :constituencyGroupEndDate ?constituencyEndDate .
    ?seat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyEndDate ?seatIncumbencyEndDate ;
        :incumbencyStartDate ?seatIncumbencyStartDate ;
        :seatIncumbencyHasHouseSeat ?seat .
}
WHERE {
    BIND(@personid AS ?person)
    ?person a :Person .
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
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
            return BaseController.ExecuteList(query);
        }

        [Route(@"{id:regex(^\w{8}$)}/constituencies/current", Name = "PersonCurrentConstituency")]
        [HttpGet]
        public Graph CurrentConstituency(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
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
    ?person a :Person .
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
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

            return BaseController.ExecuteSingle(query);
        }

        [Route(@"{id:regex(^\w{8}$)}/parties", Name = "PersonParties")]
        [HttpGet]
        public Graph Parties(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
    ?party
        a :Party ;
        :partyName ?partyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipStartDate ?partyMembershipStartDate ;
        :partyMembershipEndDate ?partyMembershipEndDate ;
        :partyMembershipHasParty ?party .
}
WHERE {
    BIND(@personid AS ?person)
    ?person a :Person .
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
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

            return BaseController.ExecuteList(query);
        }

        [Route(@"{id:regex(^\w{8}$)}/parties/current", Name = "PersonCurrentParty")]
        [HttpGet]
        public Graph CurrentParty(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
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
    ?person a :Person .
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
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

            return BaseController.ExecuteSingle(query);
        }

        // note: query currently only really returns parliamentary contact point, not "contact points"
        [Route(@"{id:regex(^\w{8}$)}/contact_points", Name = "PersonContactPoints")]
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
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
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
    ?person a :Person .
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
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

            return BaseController.ExecuteSingle(query);
        }

        [Route(@"{id:regex(^\w{8}$)}/houses", Name = "PersonHouses")]
        [HttpGet]
        public Graph Houses(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
    ?house1
        a :House ;
        :houseName ?houseName1 .
    ?house2
        a :House ;
        :houseName ?houseName2 .
    ?seatIncumbency
        a :Incumbency ;
        :incumbencyEndDate ?incumbencyEndDate ;
        :incumbencyStartDate ?incumbencyStartDate ;
        :seatIncumbencyHasHouseSeat ?houseSeat .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house1 .
    ?houseIncumbency
        a :Incumbency ;
        :incumbencyEndDate ?incumbencyEndDate ;
        :incumbencyStartDate ?incumbencyStartDate ;
        :houseIncumbencyHasHouse ?house2 .
}
WHERE {
    BIND(@personid AS ?person)
    ?person a :Person .
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
    OPTIONAL {
        ?person :memberHasIncumbency ?incumbency .
        OPTIONAL { ?incumbency :incumbencyEndDate ?incumbencyEndDate . }
        ?incumbency :incumbencyStartDate ?incumbencyStartDate .
        OPTIONAL {
            ?incumbency a :HouseIncumbency .
            BIND(?incumbency AS ?houseIncumbency )
            ?houseIncumbency :houseIncumbencyHasHouse ?house2 .
            ?house2 :houseName ?houseName2 .
        }
        OPTIONAL {
            ?incumbency a :SeatIncumbency .
            BIND(?incumbency AS ?seatIncumbency )
            ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
            ?houseSeat :houseSeatHasConstituencyGroup ?constituency .
            ?houseSeat :houseSeatHasHouse ?house1 .
            ?house1 :houseName ?houseName1 .
            ?constituency :constituencyGroupName ?constituencyName .
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, id));

            return BaseController.ExecuteList(query);
        }

        [Route(@"{id:regex(^\w{8}$)}/houses/current", Name = "PersonCurrentHouse")]
        [HttpGet]
        public Graph CurrentHouse(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
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
    ?person a :Person .
    OPTIONAL { ?person :personGivenName ?givenName } .
    OPTIONAL { ?person :personFamilyName ?familyName } .
    OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
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

            return BaseController.ExecuteSingle(query);
        }

        [Route("mps", Name = "PersonMPs")]
        [HttpGet]
        public Graph MPs()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName ;
    	:count ?memberCount .
    ?parliament
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?parliamentPeriodStartDate ;
        :parliamentPeriodEndDate ?parliamentPeriodEndDate .
    ?speaker
        a :Person .
}
WHERE {
    { SELECT ?party ?partyName (COUNT(?member) AS ?memberCount) WHERE {
    ?party a :Party ;
        :partyName ?partyName ;
    	:partyHasPartyMembership ?partyMembership .
        FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
        ?partyMembership :partyMembershipHasPartyMember ?member .
        ?member :memberHasIncumbency ?seatIncumbency .
        ?seatIncumbency a :SeatIncumbency .
        FILTER NOT EXISTS { ?seatIncumbency a :PastIncumbency . }
	}
    GROUP BY ?party ?partyName
}
    UNION {
        SELECT * WHERE {
            ?parliament a :PastParliamentPeriod ;
            			:parliamentPeriodStartDate ?parliamentPeriodStartDate ;
               			:parliamentPeriodEndDate ?parliamentPeriodEndDate .
        }
        ORDER BY DESC(?parliamentPeriodStartDate)
        LIMIT 3
    }
    UNION {
        SELECT ?speaker WHERE {
            ?speaker :partyMemberHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasParty ?party .
            ?party :partyName ""Speaker"" .
        }
    }
}
";
            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }
    }
}
