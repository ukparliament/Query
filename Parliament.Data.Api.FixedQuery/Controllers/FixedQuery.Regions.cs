namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {

        //[Route(@"postcode_lookup/{postcode}", Name = "ConstituencyLookupByPostcode")]
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
construct
{
    ?constituency a parl:ConstituencyGroup;
		parl:constituencyGroupName ?consName.   
}
where
{
    ?constituency a parl:ConstituencyGroup;
		parl:constituencyGroupName ?consName.    
    FILTER regex(?consName, @firstConstituency)
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

            query.SetLiteral("firstConstituency", constituencyRegex);
            return BaseController.ExecuteSingle(query);
        }
    }
}
