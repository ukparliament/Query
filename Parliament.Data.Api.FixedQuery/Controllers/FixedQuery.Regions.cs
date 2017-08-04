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

construct {
   ?region a admingeo:EuropeanRegion .
   ?region skos:prefLabel @region .
   ?region admingeo:westminsterConstituency ?constituency.
   ?constituency skos:prefLabel ?constituencyName.
}
WHERE {
   ?region a admingeo:EuropeanRegion .
   ?region skos:prefLabel @region .
   ?region admingeo:westminsterConstituency ?constituency.
   ?constituency skos:prefLabel ?constituencyName.
}
";
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
PREFIX admingeo: <http://data.ordnancesurvey.co.uk/ontology/admingeo/>

construct
{
    ?constituency a parl:ConstituencyGroup;
		parl:constituencyGroupName ?consName.   
}
where
{
    ?constituency a parl:ConstituencyGroup;
	    parl:constituencyGroupName ?consName.    
    FILTER regex(?consName, @constituencyRegex)
}
";
            var externalQuery = new SparqlParameterizedString(externalQueryString);
            var regionWithSpace = region + " ";
            externalQuery.SetLiteral("region", regionWithSpace);
            var externalResults = BaseController.ExecuteSingle(externalQuery, "http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql");

            var query = new SparqlParameterizedString(queryString);
            var constituenciesInRegion = externalResults.GetTriplesWithPredicate(new Uri("http://www.w3.org/2004/02/skos/core#prefLabel")).ToList();
            string constituencyRegex = String.Empty;

            foreach (var constituency in constituenciesInRegion)
            {
                constituencyRegex = constituencyRegex + "^" + constituency.Object.ToString() + "$|";
            }
            constituencyRegex = constituencyRegex.Remove(constituencyRegex.Length - 1);

            query.SetLiteral("constituencyRegex", constituencyRegex);

            var graph = BaseController.ExecuteSingle(query);
            var regionNameWithSpace = graph.CreateLiteralNode(regionWithSpace);
            var rdfType = graph.CreateUriNode(UriFactory.Create("http://rdftype.ex"));
            var label = graph.CreateUriNode(UriFactory.Create("http://www.w3.org/2004/02/skos/core#prefLabel"));
            var regionType = graph.CreateUriNode(UriFactory.Create("http://data.ordnancesurvey.co.uk/ontology/admingeo/EuropeanRegion"));


            Triple regionLabelStatement = externalResults.GetTriplesWithObject(regionType).First();
            Triple regionTypeStatement = externalResults.GetTriplesWithObject(regionNameWithSpace).First();

            graph.Assert(regionLabelStatement);
            graph.Assert(regionTypeStatement);
            return graph;
        }
    }
}
