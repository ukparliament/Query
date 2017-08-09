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
CONSTRUCT {
 ?uri skos:prefLabel ?name  .
 ?uri a admingeo:EuropeanRegion .
}
WHERE {
 ?uri skos:prefLabel ?name  .
 ?uri a admingeo:EuropeanRegion .
}
";
            var externalQuery = new SparqlParameterizedString(externalQueryString);
            return BaseController.ExecuteSingle(externalQuery, "http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql");

        }

        [HttpGet]
        public Graph region_constituencies(string region)
        {
            var externalQueryString = @"
PREFIX spatial: <http://data.ordnancesurvey.co.uk/ontology/spatialrelations/>
PREFIX skos: <http://www.w3.org/2004/02/skos/core#>
PREFIX admingeo: <http://data.ordnancesurvey.co.uk/ontology/admingeo/>

CONSTRUCT {
   ?region a admingeo:EuropeanRegion .
   ?region skos:prefLabel @region .
   ?region admingeo:westminsterConstituency ?constituency.
   ?constituency skos:prefLabel ?constituencyName.
   ?constituency admingeo:gssCode ?gss.
}
WHERE {
   ?region a admingeo:EuropeanRegion .
   ?region skos:prefLabel @region .
   ?region admingeo:westminsterConstituency ?constituency.
   ?constituency skos:prefLabel ?constituencyName.
   ?constituency admingeo:gssCode ?gss.
}
";
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
PREFIX admingeo: <http://data.ordnancesurvey.co.uk/ontology/admingeo/>

CONSTRUCT {
?constituency a parl:ConstituencyGroup;
	    parl:onsCode ?onsCode;
		parl:constituencyGroupName ?constituencyName.   
}
WHERE{
    {}
";

            var externalQuery = new SparqlParameterizedString(externalQueryString);
            var regionWithSpace = region;
            if (region[region.Length - 1].ToString() != " "){ regionWithSpace = region + " "; }
            externalQuery.SetLiteral("region", regionWithSpace);
            var externalResults = BaseController.ExecuteSingle(externalQuery, "http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql");

            var query = new SparqlParameterizedString(queryString);
            var constituenciesInRegion = externalResults.GetTriplesWithPredicate(new Uri("http://data.ordnancesurvey.co.uk/ontology/admingeo/gssCode")).ToList();
            foreach (var constituency in constituenciesInRegion)
            {
                var unionSparql = new SparqlParameterizedString("UNION {BIND(@onsCode AS ?onsCode) ?constituency a parl:ConstituencyGroup; parl:onsCode ?onsCode; parl:constituencyGroupName ?consName.} ");
                unionSparql.SetLiteral("onsCode", constituency.Object.ToString());
                query.Append(unionSparql.ToString());
            }
            query.Append("}");

            var graph = BaseController.ExecuteSingle(query);
            var regionType = graph.CreateUriNode(UriFactory.Create("http://data.ordnancesurvey.co.uk/ontology/admingeo/EuropeanRegion"));

            graph.Assert(externalResults.GetTriplesWithObject(regionType).First());
            graph.Assert(externalResults.GetTriplesWithObject(graph.CreateLiteralNode(regionWithSpace)).First());
            return graph;
        }
    }
}
