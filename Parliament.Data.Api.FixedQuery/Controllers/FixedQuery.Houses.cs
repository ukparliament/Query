namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        //[Route(@"{id:regex(^\w{8}$)}", Name = "HouseById")]
        [HttpGet]
        public Graph house_by_id(string house_id)
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

            query.SetUri("id", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteSingle(query);

        }

        //[Route(@"lookup/{source:regex(^\w+$)}/{id}", Name = "HouseLookup")]
        [HttpGet]
        public Graph house_lookup(string property, string value) => base.LookupInternal("House", property, value);

        //[Route(@"partial/{letters:regex(^\p{L}+$):minlength(2)}", Name = "HouseByLetters", Order = 999)]
        [HttpGet]
        public Graph house_by_substring(string substring)
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
    FILTER CONTAINS(LCASE(?houseName), LCASE(@substring))
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("substring", substring);

            return BaseController.ExecuteList(query);
        }

        //[Route("", Name = "HouseIndex")]
        [HttpGet]
        public Graph house_index()
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

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members", Name = "HouseMembers")]
        [HttpGet]
        public Graph house_members(string house_id)
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
        :memberHasMemberImage ?image ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?house
        a :House ;
        :houseName ?houseName .
   ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@houseid AS ?house)
        ?house a :House .

        OPTIONAL {
        ?person a :Member .
            ?incumbency
                :incumbencyHasMember ?person .
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            {
                ?incumbency :houseIncumbencyHasHouse ?house .
                OPTIONAL { ?incumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
                BIND(?incumbency AS ?houseIncumbency)
            }
            UNION {
                ?incumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                OPTIONAL { ?incumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
                ?houseSeat :houseSeatHasHouse ?house .
                BIND(?incumbency AS ?seatIncumbency)
                OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                    ?constituencyGroup :constituencyGroupName ?constituencyName .
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
                }
            }
            OPTIONAL {
                ?person :partyMemberHasPartyMembership ?partyMembership .
                FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
                OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
                ?partyMembership :partyMembershipHasParty ?party .
                ?party :partyName ?partyName .
            }
          }
       }
    }
    UNION {
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
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members/current", Name = "HouseCurrentMembers")]
        [HttpGet]
        public Graph house_current_members(string house_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?house
        a :House ;
        :houseName ?houseName .
   ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat .
    ?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup;
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
        BIND(@houseid AS ?house)
        ?house
            a :House ;
            :houseName ?houseName .
        OPTIONAL {
            ?person a :Member .
            ?incumbency
                :incumbencyHasMember ?person .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            {
                ?incumbency :houseIncumbencyHasHouse ?house .
                BIND(?incumbency AS ?houseIncumbency)
            }
            UNION {
                ?incumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                ?houseSeat :houseSeatHasHouse ?house .
                BIND(?incumbency AS ?seatIncumbency)
                OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                    ?constituencyGroup :constituencyGroupName ?constituencyName .
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
                }
            }
            OPTIONAL {
                ?person :partyMemberHasPartyMembership ?partyMembership .
                FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
                OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
                ?partyMembership :partyMembershipHasParty ?party .
                ?party :partyName ?partyName .
            }
          }
       }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)

          ?house a :House .
    	  ?person a :Member ;
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
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/parties", Name = "HouseParties")]
        [HttpGet]
        public Graph house_parties(string house_id)
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
    ?house
        a :House ;
        :houseName ?houseName .
    ?person a :Member .
    ?incumbency
        :incumbencyHasMember ?person ;
        :incumbencyStartDate ?incStartDate .
    OPTIONAL { ?incumbency :incumbencyEndDate ?incumbencyEndDate . }
    {
        ?incumbency :houseIncumbencyHasHouse ?house .
	}
    UNION {
        ?incumbency :seatIncumbencyHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasHouse ?house .
	}
    ?partyMembership
        :partyMembershipHasPartyMember ?person ;
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

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/parties/current", Name = "HouseCurrentParties")]
        [HttpGet]
        public Graph house_current_parties(string house_id)
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
    BIND(@houseid AS ?house) # Route parameter
    ?house :houseName ?houseName .
    OPTIONAL
    {
        SELECT ?party ?partyName (COUNT(?membership) AS ?memberCount)
        WHERE {
            BIND(@houseid AS ?house) # Route parameter
            # The relatioship between a house and an incumbency is either via a seat or direct (""lords bypass"")
            {
                # Commons
                ?house :houseHasHouseSeat/:houseSeatHasSeatIncumbency ?incumbency .
            }
            UNION
            {
                # Lords
                ?house :houseHasHouseIncumbency ?incumbency .
            }
            FILTER NOT EXISTS {
                ?incumbency a :PastIncumbency .
            }
            ?incumbency :incumbencyHasMember/:partyMemberHasPartyMembership ?membership .
            ?membership :partyMembershipHasParty ?party .
            ?party :partyName ?partyName .
            FILTER NOT EXISTS {
                ?membership a :PastPartyMembership .
            }
        }
        GROUP BY ?party ?partyName
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{houseId:regex(^\w{8}$)}/parties/{partyId:regex(^\w{8}$)}", Name = "HousePartyById")]
        [HttpGet]
        public Graph house_party_by_id(string house_id, string party_id)
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
    {
        SELECT *
        WHERE {
            BIND(@houseid AS ?house)
            ?house :houseName ?houseName .
            OPTIONAL {
                BIND(@partyid AS ?party)
                ?person
                	a :Member ;
                	:partyMemberHasPartyMembership ?partyMembership .
                ?partyMembership :partyMembershipHasParty ?party .
                ?party :partyName ?partyName .
                ?incumbency :incumbencyHasMember ?person .
                {
                	?incumbency :houseIncumbencyHasHouse ?house .
        		}
                UNION {
                	?incumbency :seatIncumbencyHasHouseSeat ?seat .
                	?seat :houseSeatHasHouse ?house .
        		}
        	}
        }
    }
    UNION {
    	SELECT ?party (COUNT(?currentMember) AS ?currentMemberCount)
        WHERE {
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

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));


            return BaseController.ExecuteSingle(query);
        }

        //[Route(@"{houseId:regex(^\w{8}$)}/members/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "HouseMembersByInitial")]
        [HttpGet]
        public Graph house_members_by_initial(string house_id, string initial)
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
        :memberHasIncumbency ?incumbency ;
        :memberHasMemberImage ?image ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?house
        a :House ;
        :houseName ?houseName .
   ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
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
        :partyMembershipEndDate ?partyMembershipEndDate .
    ?party
        a :Party ;
        :partyName ?partyName .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@houseid AS ?house)
        ?house
            a :House ;
            :houseName ?houseName .
        OPTIONAL {
            ?person a :Member .
            ?incumbency
                :incumbencyHasMember ?person .
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            {
                ?incumbency :houseIncumbencyHasHouse ?house .
                OPTIONAL { ?incumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
                BIND(?incumbency AS ?houseIncumbency)
            }
            UNION {
                ?incumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                OPTIONAL { ?incumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
                ?houseSeat :houseSeatHasHouse ?house .
                BIND(?incumbency AS ?seatIncumbency)
                OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                    ?constituencyGroup :constituencyGroupName ?constituencyName .
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
                }
            }
            OPTIONAL {
                ?person :partyMemberHasPartyMembership ?partyMembership .
                FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
                OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
                ?partyMembership :partyMembershipHasParty ?party .
                ?party :partyName ?partyName .
            }
            FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
         }
       }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)

          ?house a :House .
          ?person a :Member ;
                  <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
          ?incumbency :incumbencyHasMember ?person .
            {
    	        ?incumbency :houseIncumbencyHasHouse ?house .
    	    }
            UNION {
                ?incumbency :seatIncumbencyHasHouseSeat ?seat.
             	?seat :houseSeatHasHouse ?house .

            }
          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members/a_z_letters", Name = "HouseMembersAToZ")]
        [HttpGet]
        public Graph house_members_a_to_z(string house_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    _:x :value ?firstLetter .
    ?house
    	a :House ;
    	:houseName ?houseName .
}
WHERE {
	{
	    SELECT DISTINCT ?firstLetter
	    WHERE {
	        BIND(@houseid AS ?house)
	        ?house
	            a :House ;
	            :houseName ?houseName .
	        ?person
	            a :Member ;
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
	UNION
	{
		BIND(@houseid AS ?house)
        ?house
            a :House ;
            :houseName ?houseName .
	}
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{houseId:regex(^\w{8}$)}/members/current/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "HouseCurrentMembersByInitial")]
        [HttpGet]
        public Graph house_current_members_by_initial(string house_id, string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?image
        a :MemberImage .
    ?house
        a :House ;
        :houseName ?houseName .
    ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat .
    ?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
    ?constituencyGroup
        a :ConstituencyGroup;
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
        BIND(@houseid AS ?house)
        ?house
            a :House ;
            :houseName ?houseName .
        OPTIONAL {
            ?person a :Member .
            ?incumbency
                :incumbencyHasMember ?person .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            {
                ?incumbency :houseIncumbencyHasHouse ?house .
                BIND(?incumbency AS ?houseIncumbency)
            }
            UNION {
                ?incumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                ?houseSeat :houseSeatHasHouse ?house .
                BIND(?incumbency AS ?seatIncumbency)
                OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                    ?constituencyGroup :constituencyGroupName ?constituencyName .
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
                }
            }
            OPTIONAL {
                ?person :partyMemberHasPartyMembership ?partyMembership .
                FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
                OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
                ?partyMembership :partyMembershipHasParty ?party .
                ?party :partyName ?partyName .
            }
            FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
        }
       }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)

          ?house a :House .
          ?person a :Member ;
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
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members/current/a_z_letters", Name = "HouseCurrentMembersAToZ")]
        [HttpGet]
        public Graph house_current_members_a_to_z(string house_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    _:x :value ?firstLetter .
    ?house
    	a :House ;
    	:houseName ?houseName .
}
WHERE {
	{
	    SELECT DISTINCT ?firstLetter
	    WHERE {
	        BIND(@houseid AS ?house)
	        ?house
	            a :House ;
	            :houseName ?houseName .
	        ?person
	            a :Member;
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
    UNION
    {
    	BIND(@houseid AS ?house)
        ?house
            a :House ;
            :houseName ?houseName .
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{houseId:regex(^\w{8}$)}/parties/{partyId:regex(^\w{8}$)}/members", Name = "HousePartyMembers")]
        [HttpGet]
        public Graph house_party_members(string house_id, string party_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :partyMemberHasPartyMembership ?partyMembership ;
        :memberHasIncumbency ?incumbency .
    ?image
        a :MemberImage .
    ?house
        a :House ;
        :houseName ?houseName .
   ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
   ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
    ?party
        a :Party ;
        :partyName ?partyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@houseid AS ?house)
        ?house a :House ;
               :houseName ?houseName .
         OPTIONAL {
            BIND(@partyid AS ?party)
            ?party a :Party ;
                   :partyName ?partyName .
            ?person
                a :Member ;
                :partyMemberHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasParty ?party .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
            ?incumbency
                :incumbencyHasMember ?person .
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            {
                ?incumbency :houseIncumbencyHasHouse ?house .
                OPTIONAL { ?incumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
                BIND(?incumbency AS ?houseIncumbency)
            }
            UNION {
                ?incumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                OPTIONAL { ?incumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
                ?houseSeat :houseSeatHasHouse ?house .
                BIND(?incumbency AS ?seatIncumbency)
                OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                    ?constituencyGroup :constituencyGroupName ?constituencyName .
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
                }
            }
		}
       }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)
          BIND(@partyid AS ?party)

          ?house a :House .
          ?party a :Party .
    	  ?person a :Member ;
          		<http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
    	    	:partyMemberHasPartyMembership ?partyMembership .
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
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{houseId:regex(^\w{8}$)}/parties/{partyId:regex(^\w{8}$)}/members/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "HousePartyMembersByInitial")]
        [HttpGet]
        public Graph house_party_members_by_initial(string house_id, string party_id, string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :partyMemberHasPartyMembership ?partyMembership ;
        :memberHasIncumbency ?incumbency .
    ?image
        a :MemberImage .
    ?house
        a :House ;
        :houseName ?houseName .
   ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat ;
        :incumbencyEndDate ?seatIncumbencyEndDate .
    ?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house ;
        :incumbencyEndDate ?houseIncumbencyEndDate .
   ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
    ?party
        a :Party ;
        :partyName ?partyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party ;
        :partyMembershipEndDate ?partyMembershipEndDate .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@houseid AS ?house)
        ?house a :House ;
               :houseName ?houseName .
         OPTIONAL {
            BIND(@partyid AS ?party)
            ?party a :Party ;
                   :partyName ?partyName .
            ?person
                a :Member ;
                :partyMemberHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasParty ?party .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
            ?incumbency
                :incumbencyHasMember ?person .
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            {
                ?incumbency :houseIncumbencyHasHouse ?house .
                OPTIONAL { ?incumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
                BIND(?incumbency AS ?houseIncumbency)
            }
            UNION {
                ?incumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                OPTIONAL { ?incumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
                ?houseSeat :houseSeatHasHouse ?house .
                BIND(?incumbency AS ?seatIncumbency)
                OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                    ?constituencyGroup :constituencyGroupName ?constituencyName .
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
                }
            }
            FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
          }
        }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)
          BIND(@partyid AS ?party)

          ?house a :House .
          ?party a :Party .
          ?person a :Member ;
          		<http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
    	    	:partyMemberHasPartyMembership ?partyMembership.
    	    ?partyMembership :partyMembershipHasParty ?party .
    	    ?incumbency :incumbencyHasMember ?person .

            {
    	        ?incumbency :houseIncumbencyHasHouse ?house .
            }
            UNION {
                ?incumbency :seatIncumbencyHasHouseSeat ?seat.
             	?seat :houseSeatHasHouse ?house .
    	    }

          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));
            query.SetLiteral("initial", initial);


            return BaseController.ExecuteList(query);
        }

        //[Route(@"{houseId:regex(^\w{8}$)}/parties/{partyId:regex(^\w{8}$)}/members/a_z_letters", Name = "HousePartyMembersAToZ")]
        [HttpGet]
        public Graph house_party_members_a_to_z(string house_id, string party_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    [ :value ?firstLetter ] .
    @houseid
        a :House ;
        :houseName ?houseName .
    @partyid
        a :Party ;
        :partyName ?partyName .
}
WHERE {
    SELECT DISTINCT ?houseName ?partyName ?firstLetter
    WHERE {
        BIND(@houseid AS ?house)
        BIND(@partyid AS ?party)

        ?house
            :houseName ?houseName .
        ?party
            :partyName ?partyName .
        ?person
            a :Member ;
            <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
            :partyMemberHasPartyMembership ?partyMembership .
        ?partyMembership :partyMembershipHasParty ?party .
        ?incumbency :incumbencyHasMember ?person .

        {
            ?incumbency :houseIncumbencyHasHouse ?house .
        }
        UNION {
            ?incumbency :seatIncumbencyHasHouseSeat ?seat .
            ?seat :houseSeatHasHouse ?house .
        }

        BIND(UCASE(SUBSTR(?listAs, 1, 1)) AS ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{houseId:regex(^\w{8}$)}/parties/{partyId:regex(^\w{8}$)}/members/current", Name = "HousePartyCurrentMembers")]
        [HttpGet]
        public Graph house_party_current_members(string house_id, string party_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :partyMemberHasPartyMembership ?partyMembership ;
        :memberHasIncumbency ?incumbency .
    ?image
        a :MemberImage .
    ?house
        a :House ;
        :houseName ?houseName .
   ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat .
    ?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house .
   ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
    ?party
        a :Party ;
        :partyName ?partyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@houseid AS ?house)
        BIND(@partyid AS ?party)
        ?house a :House ;
               :houseName ?houseName .
        ?party a :Party ;
               :partyName ?partyName .
         OPTIONAL {
            ?person
                a :Member ;
                :partyMemberHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasParty ?party .
            ?incumbency :incumbencyHasMember ?person .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            {
                ?incumbency :houseIncumbencyHasHouse ?house .
                BIND(?incumbency AS ?houseIncumbency)
            }
            UNION {
                ?incumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                ?houseSeat :houseSeatHasHouse ?house .
                BIND(?incumbency AS ?seatIncumbency)
                OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                    ?constituencyGroup :constituencyGroupName ?constituencyName .
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
                }
            }
         }
      }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)
          BIND(@partyid AS ?party)

          ?house a :House.
          ?party a :Party.
          ?person a :Member ;
          		<http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
    	    	:partyMemberHasPartyMembership ?partyMembership.
          FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership. }
            ?partyMembership :partyMembershipHasParty ?party.
     	    ?incumbency :incumbencyHasMember ?person .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency. }
    	    {
                ?incumbency :houseIncumbencyHasHouse ?house.
    	    }
    	    UNION {
                ?incumbency :seatIncumbencyHasHouseSeat ?seat.
             	?seat :houseSeatHasHouse ?house .
    	    }
          BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
      }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{houseId:regex(^\w{8}$)}/parties/{partyId:regex(^\w{8}$)}/members/current/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "HousePartyCurrentMembersByInitial")]
        [HttpGet]
        public Graph house_party_current_members_by_initial(string house_id, string party_id, string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        :memberHasMemberImage ?image ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :partyMemberHasPartyMembership ?partyMembership ;
        :memberHasIncumbency ?incumbency .
    ?image
        a :MemberImage .
    ?house
        a :House ;
        :houseName ?houseName .
   ?seatIncumbency
        a :SeatIncumbency ;
        :seatIncumbencyHasHouseSeat ?houseSeat .
    ?houseIncumbency
        a :HouseIncumbency ;
        :houseIncumbencyHasHouse ?house .
   ?houseSeat
        a :HouseSeat ;
        :houseSeatHasHouse ?house ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
   ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
    ?party
        a :Party ;
        :partyName ?partyName .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipHasParty ?party .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@houseid AS ?house)
        BIND(@partyid AS ?party)
        ?house a :House ;
               :houseName ?houseName .
        ?party a :Party ;
               :partyName ?partyName .
         OPTIONAL {
            ?person
                a :Member ;
                :partyMemberHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasParty ?party .
            ?incumbency :incumbencyHasMember ?person .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person :memberHasMemberImage ?image . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
            {
                ?incumbency :houseIncumbencyHasHouse ?house .
                BIND(?incumbency AS ?houseIncumbency)
            }
            UNION {
                ?incumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                ?houseSeat :houseSeatHasHouse ?house .
                BIND(?incumbency AS ?seatIncumbency)
                OPTIONAL { ?houseSeat :houseSeatHasConstituencyGroup ?constituencyGroup .
                    ?constituencyGroup :constituencyGroupName ?constituencyName .
                    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
                }
            }
            FILTER STRSTARTS(LCASE(?listAs), LCASE(@initial))
         }
      }
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
          BIND(@houseid AS ?house)
          BIND(@partyid AS ?party)

          ?house a :House .
          ?party a :Party .
          ?person a :Member ;
          		<http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
    	    	:partyMemberHasPartyMembership ?partyMembership .
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
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));
            query.SetLiteral("initial", initial);


            return BaseController.ExecuteList(query);
        }

        //[Route(@"{houseId:regex(^\w{8}$)}/parties/{partyId:regex(^\w{8}$)}/members/current/a_z_letters", Name = "HousePartyCurrentMembersAToZ")]
        [HttpGet]
        public Graph house_party_current_members_a_to_z(string house_id, string party_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    [ :value ?firstLetter ] .
    @houseid
        a :House ;
        :houseName ?houseName .
    @partyid
        a :Party ;
        :partyName ?partyName .
}
WHERE {
    SELECT DISTINCT ?houseName ?partyName ?firstLetter
    WHERE {
        BIND(@houseid AS ?house)
        BIND(@partyid AS ?party)

        ?house
            :houseName ?houseName .
        ?party
            :partyName ?partyName .
        ?person
            a :Member ;
            <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
            :partyMemberHasPartyMembership ?partyMembership .
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

        BIND(UCASE(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }
    }
}
