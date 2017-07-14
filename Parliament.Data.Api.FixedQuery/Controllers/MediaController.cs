namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("media")]
    public class MediaController : BaseController
    {
        [Route(@"{id:regex(^\w{8}$)}", Name = "ImageById")]
        [HttpGet]
        public Graph ById(string id)
        {
            var queryString = @"
    PREFIX : <http://id.ukpds.org/schema/>
    CONSTRUCT {
    ?image
        a :MemberImage ;
        :memberImageHasMember ?person .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs ;
        <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
    }
    WHERE {
        BIND(@id AS ?image)
        ?image
            a :MemberImage ;
            :memberImageHasMember ?person .
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
        ?person <http://example.com/A5EE13ABE03C4D3A8F1A274F57097B6C> ?listAs .
    }
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, id));

            return BaseController.ExecuteSingle(query);
        }
    }
}