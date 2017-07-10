namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class XController 
    {
        //[Route("", Name = "PartyIndex")]
        [HttpGet]
        public Graph PartyIndex()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName ;
        :commonsCount ?commonsCount ;
        :lordsCount ?lordsCount .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT ?party ?partyName (COUNT(?mp) AS ?commonsCount) (COUNT(?lord) AS ?lordsCount) WHERE {
        ?party
            a :Party ;
            :partyHasPartyMembership ?partyMembership ;
            :partyName ?partyName .
        OPTIONAL {
            ?partyMembership :partyMembershipHasPartyMember ?person .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?person :memberHasIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL {
			    ?incumbency a :SeatIncumbency ;
                			:incumbencyHasMember ?mp .
            }
            OPTIONAL {
			    ?incumbency a :HouseIncumbency ;
                  			:incumbencyHasMember ?lord .
            }
        }
      }
      GROUP BY ?party ?partyName
   }
   UNION {
        SELECT DISTINCT ?firstLetter WHERE {
	        ?s a :Party ;
                :partyHasPartyMembership ?partyMembership ;
            	:partyName ?partyName .

          	BIND(ucase(SUBSTR(?partyName, 1, 1)) as ?firstLetter)
         }
   }
}
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}", Name = "PartyById")]
        [HttpGet]
        public Graph PartyById(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party 
        a :Party ;
        :partyName ?name ;
        :commonsCount ?commonsCount ;
        :lordsCount ?lordsCount .
}
WHERE {
    SELECT ?party ?name (COUNT(?mp) AS ?commonsCount) (COUNT(?lord) AS ?lordsCount)
    WHERE {
        BIND(@id AS ?party)
       	?party a :Party ;
	           :partyName ?name .
        OPTIONAL {
            ?party :partyHasPartyMembership ?partyMembership .
    	  	FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
    	  	?partyMembership :partyMembershipHasPartyMember ?member .
    	  	?member :memberHasIncumbency ?incumbency .
    	  	FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL {
		        ?incumbency a :SeatIncumbency ;
                			:incumbencyHasMember ?mp .
            }
            OPTIONAL {
                ?incumbency a :HouseIncumbency ;
                            :incumbencyHasMember ?lord .
            }
        }
    }
    GROUP BY ?party ?name
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, id));

            return BaseController.ExecuteSingle(query);

        }

        //[Route(@"{initial:regex(^\p{L}+$):maxlength(1)}", Name = "PartyByInitial")]
        [HttpGet]
        public Graph PartyByInitial(string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName ;
        :commonsCount ?commonsCount ;
        :lordsCount ?lordsCount .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT ?party ?partyName (COUNT(?mp) AS ?commonsCount) (COUNT(?lord) AS ?lordsCount) WHERE {
        ?party
            a :Party ;
            :partyHasPartyMembership ?partyMembership ;
            :partyName ?partyName .
        OPTIONAL {
            ?partyMembership :partyMembershipHasPartyMember ?person .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?person :memberHasIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL {
			    ?incumbency a :SeatIncumbency ;
                  			:incumbencyHasMember ?mp .
            }
            OPTIONAL {
			    ?incumbency a :HouseIncumbency ;
                  		    :incumbencyHasMember ?lord .
            }
        }
        FILTER STRSTARTS(LCASE(?partyName), LCASE(@initial)) .
     }
     GROUP BY ?party ?partyName
    }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {
            ?s a :Party ;
                :partyHasPartyMembership ?partyMembership;
                :partyName ?partyName.

            BIND(ucase(SUBSTR(?partyName, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        //[Route("current", Name = "PartyCurrent")]
        [HttpGet]
        public Graph PartyCurrent()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName ;
        :commonsCount ?commonsCount ;
        :lordsCount ?lordsCount .
}
WHERE {
    SELECT ?party ?partyName (COUNT(?mp) AS ?commonsCount) (COUNT(?lord) AS ?lordsCount) WHERE {
        ?incumbency a :Incumbency .
        FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
        ?incumbency :incumbencyHasMember ?person .
        ?person :partyMemberHasPartyMembership ?partyMembership .
        FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
        ?partyMembership :partyMembershipHasParty ?party .
        ?party :partyName ?partyName .
        OPTIONAL {
	        ?incumbency a :SeatIncumbency ;
                	    :incumbencyHasMember ?mp .
        }
        OPTIONAL {
		    ?incumbency a :HouseIncumbency ;
                		:incumbencyHasMember ?lord .
        }
    }
    GROUP BY ?party ?partyName
}
";
            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        //[Route("a_z_letters", Name = "PartyAToZ")]
        [HttpGet]
        public Graph PartyAToZ()
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
            a :Party ;
           :partyHasPartyMembership ?partyMembership ;
           :partyName ?partyName .
        BIND(ucase(SUBSTR(?partyName, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }

        //[Route("current/a_z_letters", Name = "PartyCurrentAToZ")]
        [HttpGet]
        public Graph PartyCurrentAToZ()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    [ :value ?firstLetter ]
}
WHERE {
    SELECT DISTINCT ?firstLetter 
    WHERE {
        ?incumbency a :Incumbency .
        FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
        ?incumbency :incumbencyHasMember ?person .
        ?person :partyMemberHasPartyMembership ?partyMembership .
        FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
        ?partyMembership :partyMembershipHasParty ?party .
        ?party :partyName ?partyName .
        BIND(ucase(SUBSTR(?partyName, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }

        //[Route(@"lookup/{source:regex(^\w+$)}/{id}", Name = "PartyLookup")]
        [HttpGet]
        public Graph PartyLookup(string source, string id) => base.LookupInternal("Party", source, id);

        //[Route(@"partial/{letters:regex(^\p{L}+$):minlength(2)}", Name = "PartyByLetters", Order = 999)]
        [HttpGet]
        public Graph PartyByLetters(string letters)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName ;
        :commonsCount ?commonsCount ;
        :lordsCount ?lordsCount .
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT ?party ?partyName (COUNT(?mp) AS ?commonsCount) (COUNT(?lord) AS ?lordsCount) WHERE {
        ?party
            a :Party ;
            :partyHasPartyMembership ?partyMembership ;
            :partyName ?partyName .
        OPTIONAL {
            ?partyMembership :partyMembershipHasPartyMember ?person .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?person :memberHasIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL {
			    ?incumbency a :SeatIncumbency ;
                			:incumbencyHasMember ?mp .
            }
            OPTIONAL {
			    ?incumbency a :HouseIncumbency ;
                  			:incumbencyHasMember ?lord .
            }
        }
        FILTER CONTAINS(LCASE(?partyName), LCASE(@letters))

      }
        GROUP BY ?party ?partyName
   }
    UNION {
        SELECT DISTINCT ?firstLetter WHERE {

            ?s a :Party ;
                :partyHasPartyMembership ?partyMembership;
            	:partyName ?partyName.

              BIND(ucase(SUBSTR(?partyName, 1, 1)) as ?firstLetter)
         }
   }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letters", letters);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members", Name = "PartyMembers")]
        [HttpGet]
        public Graph PartyMembers(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party 
        a :Party ;
        :partyName ?partyName .
    ?person 
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership 
        a :PartyMembership ;
        :partyMembershipEndDate ?partyMembershipEndDate ;
        :partyMembershipHasParty ?party .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
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
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@partyid AS ?party)
        ?party 
            a :Party ;
            :partyName ?partyName .
        OPTIONAL {
            ?party :partyHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasPartyMember ?person .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
                	:memberHasIncumbency ?incumbency .
            OPTIONAL
            {
                ?incumbency a :HouseIncumbency .
                BIND(?incumbency AS ?houseIncumbency)
                OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        	}
        	OPTIONAL {
                ?incumbency a :SeatIncumbency .
                BIND(?incumbency AS ?seatIncumbency)
                ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
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
            BIND(@partyid AS ?party)

            ?party a :Party ;
                   :partyHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasPartyMember ?person .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

            BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members/current", Name = "PartyCurrentMembers")]
        [HttpGet]
        public Graph PartyCurrentMembers(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipEndDate ?partyMembershipEndDate ;
        :partyMembershipHasParty ?party .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
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
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@partyid AS ?party)
        ?party
            a :Party ;
            :partyName ?partyName .
        OPTIONAL {
            ?party :partyHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasPartyMember ?person .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
                	:memberHasIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL
            {
                ?incumbency a :HouseIncumbency .
                BIND(?incumbency AS ?houseIncumbency)
                OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        	}
        	OPTIONAL {
                ?incumbency a :SeatIncumbency .
                BIND(?incumbency AS ?seatIncumbency)
                ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
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
            BIND(@partyid AS ?party)

            ?party a :Party ;
                   :partyHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasPartyMember ?person .
            ?person :memberHasIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

            BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "PartyMembersByInitial")]
        [HttpGet]
        public Graph PartyMembersByInitial(string id, string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party 
        a :Party ;
        :partyName ?partyName .
    ?person 
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership 
        a :PartyMembership ;
        :partyMembershipEndDate ?endDate ;
        :partyMembershipHasParty ?party .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
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
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@partyid AS ?party)
        ?party 
            a :Party ;
            :partyName ?partyName .
        OPTIONAL {
            ?party :partyHasPartyMembership ?partyMembership .
            ?partyMembership :partyMembershipHasPartyMember ?person .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?endDate . }
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
                	:memberHasIncumbency ?incumbency .
            OPTIONAL
            {
                ?incumbency a :HouseIncumbency .
                BIND(?incumbency AS ?houseIncumbency)
                OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        	}
        	OPTIONAL {
                ?incumbency a :SeatIncumbency .
                BIND(?incumbency AS ?seatIncumbency)
                ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
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
            BIND(@partyid AS ?party)

            ?party a :Party ;
                   :partyHasPartyMembership ?partyMembership.
            ?partyMembership :partyMembershipHasPartyMember ?person .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

            BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members/a_z_letters", Name = "PartyMembersAToZ")]
        [HttpGet]
        public Graph PartyMembersAToZ(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    [ :value ?firstLetter ]
}
WHERE {
    SELECT DISTINCT ?firstLetter 
    WHERE {
        BIND(@partyid AS ?party)
        ?party 
            a :Party ;
            :partyHasPartyMembership ?partyMembership .
        ?partyMembership :partyMembershipHasPartyMember ?person .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, id));

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members/current/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "PartyCurrentMembersByInitial")]
        [HttpGet]
        public Graph PartyCurrentMembersByInitial(string id, string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?party
        a :Party ;
        :partyName ?partyName .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
        :memberHasIncumbency ?incumbency ;
        :partyMemberHasPartyMembership ?partyMembership .
    ?partyMembership
        a :PartyMembership ;
        :partyMembershipEndDate ?partyMembershipEndDate ;
        :partyMembershipHasParty ?party .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasConstituencyGroup ?constituencyGroup .
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
    _:x :value ?firstLetter .
}
WHERE {
    { SELECT * WHERE {
        BIND(@partyid AS ?party)
        ?party
            a :Party ;
            :partyName ?partyName .
        OPTIONAL {
            ?party :partyHasPartyMembership ?partyMembership .
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
            ?partyMembership :partyMembershipHasPartyMember ?person .
            OPTIONAL { ?partyMembership :partyMembershipEndDate ?partyMembershipEndDate . }
            OPTIONAL { ?person :personGivenName ?givenName . }
            OPTIONAL { ?person :personFamilyName ?familyName . }
            OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs ;
                	:memberHasIncumbency ?incumbency .
            FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
            OPTIONAL
            {
                ?incumbency a :HouseIncumbency .
                BIND(?incumbency AS ?houseIncumbency)
                OPTIONAL { ?houseIncumbency :incumbencyEndDate ?houseIncumbencyEndDate . }
        	}
        	OPTIONAL {
                ?incumbency a :SeatIncumbency .
                BIND(?incumbency AS ?seatIncumbency)
                ?seatIncumbency :seatIncumbencyHasHouseSeat ?houseSeat .
                OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
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
            BIND(@partyid AS ?party)

            ?party a :Party ;
                   :partyHasPartyMembership ?partyMembership.
            FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership. }
            ?partyMembership :partyMembershipHasPartyMember ?person.
            ?person :memberHasIncumbency ?incumbency .

             FILTER NOT EXISTS { ?incumbency a :PastIncumbency. }
            ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .

            BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}/members/current/a_z_letters", Name = "PartyCurrentMembersAToZ")]
        [HttpGet]
        public Graph PartyCurrentMembersAToZ(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    [ :value ?firstLetter ]
}
WHERE {
    SELECT DISTINCT ?firstLetter 
    WHERE {
        BIND(@partyid AS ?party)
        ?party 
            a :Party ;
            :partyHasPartyMembership ?partyMembership .
        FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
        ?partyMembership :partyMembershipHasPartyMember ?person .
        ?person :memberHasIncumbency ?incumbency .
        FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
        BIND(ucase(SUBSTR(?listAs, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, id));

            return BaseController.ExecuteList(query);
        }
    }
}
