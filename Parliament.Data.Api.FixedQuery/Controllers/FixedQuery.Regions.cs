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
    }
}
