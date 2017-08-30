namespace Parliament.Data.Api.FixedQuery.Controllers
{
  using System;
  using System.Linq;
  using System.Web.Http;
  using VDS.RDF;
  using VDS.RDF.Query;

  public partial class FixedQueryController
  {
    [HttpGet]
    public Graph region_index()
    {
      var externalQueryString = @"
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
      var externalQuery = new SparqlParameterizedString(externalQueryString);
      return BaseController.ExecuteSingle(externalQuery, "http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql");
    }

    [HttpGet]
    public Graph region_by_id(string region_code)
    {
      var queryString = @"
      PREFIX admingeo: <http://data.ordnancesurvey.co.uk/ontology/admingeo/>
      PREFIX skos: <http://www.w3.org/2004/02/skos/core#>
      PREFIX : <http://id.ukpds.org/schema/>
      CONSTRUCT {
        ?region
          a admingeo:EuropeanRegion ;
          admingeo:gssCode ?regionCode ;
          skos:prefLabel ?label .
        ?constituency
          a :ConstituencyGroup;
          :constituencyGroupName ?constituencyName.
      }
      WHERE {
        SERVICE <http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql> {
          BIND (@regionCode AS ?regionCode)
          ?region
            a admingeo:EuropeanRegion ;
            admingeo:gssCode ?regionCode ;
            skos:prefLabel ?label ;
            admingeo:westminsterConstituency/admingeo:gssCode ?onsCode.
        }
        ?constituency
          :onsCode ?onsCode;
          :constituencyGroupName ?constituencyName.
      }";
      var query = new SparqlParameterizedString(queryString);
      query.SetLiteral("regionCode", region_code);
      return BaseController.ExecuteSingle(query);
    }

    [HttpGet]
    public Graph region_constituencies(string region_code)
    {
      var queryString = @"
      PREFIX admingeo: <http://data.ordnancesurvey.co.uk/ontology/admingeo/>
      PREFIX skos: <http://www.w3.org/2004/02/skos/core#>
      PREFIX : <http://id.ukpds.org/schema/>

      CONSTRUCT {
        ?region
          a admingeo:EuropeanRegion ;
          admingeo:gssCode ?regionCode ;
          skos:prefLabel ?regionName ;
          :count ?count .
        ?constituency
          a :ConstituencyGroup;
          :constituencyGroupName ?constituencyName ;
          :constituencyGroupHasHouseSeat ?houseSeat .
        ?houseSeat
          a :HouseSeat ;
          :houseSeatHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency a :SeatIncumbency ;
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
        {
          SERVICE <http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql> {
            SELECT ?region (COUNT(?constituency) AS ?count) WHERE {
              BIND (@regionCode AS ?regionCode)
              ?region
                a admingeo:EuropeanRegion ;
                admingeo:gssCode ?regionCode ;
                admingeo:westminsterConstituency ?constituency .
            } GROUP BY ?region
          }

          SERVICE <http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql> {
            BIND (@regionCode AS ?regionCode)
            ?region
              a admingeo:EuropeanRegion ;
              admingeo:gssCode ?regionCode ;
              skos:prefLabel ?regionName ;
              admingeo:westminsterConstituency/admingeo:gssCode ?onsCode.
          }

          ?constituency
            :onsCode ?onsCode;
            :constituencyGroupName ?constituencyName ;
            :constituencyGroupHasHouseSeat ?houseSeat .
          ?houseSeat :houseSeatHasSeatIncumbency ?seatIncumbency .
          ?seatIncumbency a :SeatIncumbency ;
          FILTER NOT EXISTS { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
          OPTIONAL { ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate . }
          OPTIONAL {
            ?seatIncumbency :incumbencyHasMember ?member .
            OPTIONAL { ?member :personGivenName ?givenName . }
            OPTIONAL { ?member :personFamilyName ?familyName . }
            OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs . }
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

        UNION {

          SELECT DISTINCT ?firstLetter WHERE {
            SERVICE <http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql> {
            BIND (@regionCode AS ?regionCode)
            ?region
              a admingeo:EuropeanRegion ;
              admingeo:gssCode ?regionCode ;
              admingeo:westminsterConstituency/admingeo:gssCode ?onsCode.
            }
            ?constituency
              a :ConstituencyGroup ;
              :onsCode ?onsCode;
              :constituencyGroupName ?constituencyName .
            BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
          }
        }
      }";
      var query = new SparqlParameterizedString(queryString);
      query.SetLiteral("regionCode", region_code);
      return BaseController.ExecuteSingle(query);
    }

    [HttpGet]
    public Graph region_constituencies_a_to_z(string region_code)
    {
      var queryString = @"
      PREFIX admingeo: <http://data.ordnancesurvey.co.uk/ontology/admingeo/>
      PREFIX skos: <http://www.w3.org/2004/02/skos/core#>
      PREFIX : <http://id.ukpds.org/schema/>

      CONSTRUCT {
        ?region
          a admingeo:EuropeanRegion ;
          admingeo:gssCode ?regionCode ;
          skos:prefLabel ?regionName .
        _:x :value ?firstLetter .
      }
      WHERE {
        { SERVICE <http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql> {
          BIND (@regionCode AS ?regionCode)
          ?region
            a admingeo:EuropeanRegion ;
            admingeo:gssCode ?regionCode ;
            skos:prefLabel ?regionName ;
            admingeo:westminsterConstituency/admingeo:gssCode ?onsCode.
      	  }
        }
        UNION {
          SELECT DISTINCT ?firstLetter WHERE {
      	  SERVICE <http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql> {
	        BIND (@regionCode AS ?regionCode)
	        ?region
	          a admingeo:EuropeanRegion ;
	          admingeo:gssCode ?regionCode ;
	          admingeo:westminsterConstituency/admingeo:gssCode ?onsCode.
      	  }
            ?constituency
              a :ConstituencyGroup ;
              :onsCode ?onsCode;
              :constituencyGroupName ?constituencyName .
            BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
          }
        }
      }
      ";
      var query = new SparqlParameterizedString(queryString);
      query.SetLiteral("regionCode", region_code);
      return BaseController.ExecuteSingle(query);
    }

    [HttpGet]
    public Graph region_constituencies_by_initial(string region_code, string initial)
    {
      var queryString = @"
      PREFIX admingeo: <http://data.ordnancesurvey.co.uk/ontology/admingeo/>
      PREFIX skos: <http://www.w3.org/2004/02/skos/core#>
      PREFIX : <http://id.ukpds.org/schema/>

      CONSTRUCT {
        ?region
          a admingeo:EuropeanRegion ;
          admingeo:gssCode ?regionCode ;
          skos:prefLabel ?regionName ;
          :count ?count .
        ?constituency
          a :ConstituencyGroup;
          :constituencyGroupName ?constituencyName ;
          :constituencyGroupHasHouseSeat ?houseSeat .
        ?houseSeat
          a :HouseSeat ;
          :houseSeatHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency a :SeatIncumbency ;
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
          SERVICE <http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql> {
            SELECT ?region (COUNT(?constituency) AS ?count) WHERE {
              BIND (@regionCode AS ?regionCode)
              ?region
                a admingeo:EuropeanRegion ;
                admingeo:gssCode ?regionCode ;
                admingeo:westminsterConstituency ?constituency .
            } GROUP BY ?region
          }
          SERVICE <http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql> {
            BIND (@regionCode AS ?regionCode)
            ?region
              a admingeo:EuropeanRegion ;
              admingeo:gssCode ?regionCode ;
              skos:prefLabel ?regionName ;
              admingeo:westminsterConstituency/admingeo:gssCode ?onsCode.
          }
          ?constituency
            :onsCode ?onsCode;
            :constituencyGroupName ?constituencyName ;
            :constituencyGroupHasHouseSeat ?houseSeat .
          ?houseSeat :houseSeatHasSeatIncumbency ?seatIncumbency .
          ?seatIncumbency a :SeatIncumbency ;
          FILTER NOT EXISTS { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
          OPTIONAL { ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate . }
          OPTIONAL {
            ?seatIncumbency :incumbencyHasMember ?member .
            OPTIONAL { ?member :personGivenName ?givenName . }
            OPTIONAL { ?member :personFamilyName ?familyName . }
            OPTIONAL { ?member <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs . }
            OPTIONAL {
              ?member :partyMemberHasPartyMembership ?partyMembership .
              FILTER NOT EXISTS { ?partyMembership a :PastPartyMembership . }
              OPTIONAL {
                ?partyMembership :partyMembershipHasParty ?party .
                OPTIONAL { ?party :partyName ?partyName . }
              }
            }
          }
          FILTER STRSTARTS(LCASE(?constituencyName), LCASE(@initial))
        }
      }
      UNION {
        SELECT DISTINCT ?firstLetter WHERE {
    	    SERVICE <http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql> {
		      BIND (@regionCode AS ?regionCode)
		      ?region
		        a admingeo:EuropeanRegion ;
		        admingeo:gssCode ?regionCode ;
		        admingeo:westminsterConstituency/admingeo:gssCode ?onsCode.
	        }
            ?constituency
              a :ConstituencyGroup ;
              :onsCode ?onsCode;
              :constituencyGroupName ?constituencyName .
            BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
          }
        }
      }
      ";
      var query = new SparqlParameterizedString(queryString);
      query.SetLiteral("regionCode", region_code);
      query.SetLiteral("initial", initial);
      return BaseController.ExecuteSingle(query);
    }
  }
}
