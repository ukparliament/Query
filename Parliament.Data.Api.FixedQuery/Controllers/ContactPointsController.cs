namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Net.Http;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("contact_points")]
    public class ContactPointsController : BaseController
    {
        // Ruby route: resources :contact_points, only: [:index]
        [Route("", Name = "ContactPointIndex")]
        [HttpGet]
        public Graph Index()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?contactPoint
        a :ContactPoint ;
        :email ?email ;
        :phoneNumber ?phoneNumber ;
        :faxNumber ?faxNumber ;
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
    ?contactPoint a :ContactPoint .
    OPTIONAL{ ?contactPoint :email ?email . }
    OPTIONAL{ ?contactPoint :phoneNumber ?phoneNumber . }
    OPTIONAL{ ?contactPoint :faxNumber ?faxNumber . }
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
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.Execute(query);
        }

        // Ruby route: resources :contact_points, only: [:show]
        [Route(@"{id:regex(^\w{8}$)}", Name = "ContactPointById")]
        [HttpGet]
        public Graph ById(string id)
        {
            var queryString = @"
PREFIX :<http://id.ukpds.org/schema/>
CONSTRUCT {
    ?contactPoint
        a :ContactPoint ;
        :email ?email ;
        :phoneNumber ?phoneNumber ;
        :faxNumber ?faxNumber ;
        :contactPointHasPostalAddress ?postalAddress ;
        :contactPointHasIncumbency ?incumbency .
    ?incumbency
        a :Incumbency ;
        :incumbencyHasMember ?person .
    ?postalAddress
        a :PostalAddress ;
        :postCode ?postCode ;
        :addressLine1 ?addressLine1 ;
        :addressLine2 ?addressLine2 ;
        :addressLine3 ?addressLine3 ;
        :addressLine4 ?addressLine4 ;
        :addressLine5 ?addressLine5 .
    ?person
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName ;
        <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
}
WHERE {
    BIND(@id AS ?contactPoint )
    ?contactPoint a :ContactPoint .
    OPTIONAL{ ?contactPoint :email ?email . }
    OPTIONAL{ ?contactPoint :phoneNumber ?phoneNumber . }
    OPTIONAL{ ?contactPoint :faxNumber ?faxNumber . }
    OPTIONAL{
        ?contactPoint :contactPointHasPostalAddress ?postalAddress .
        OPTIONAL{ ?postalAddress :postCode ?postCode . }
        OPTIONAL{ ?postalAddress :addressLine1 ?addressLine1 . }
        OPTIONAL{ ?postalAddress :addressLine2 ?addressLine2 . }
        OPTIONAL{ ?postalAddress :addressLine3 ?addressLine3 . }
        OPTIONAL{ ?postalAddress :addressLine4 ?addressLine4 . }
        OPTIONAL{ ?postalAddress :addressLine5 ?addressLine5 . }
	}
    OPTIONAL{
        ?contactPoint :contactPointHasIncumbency ?incumbency .
        ?incumbency :incumbencyHasMember ?person .
        OPTIONAL { ?person :personFamilyName ?familyName . }
        OPTIONAL { ?person :personGivenName ?givenName . }
        OPTIONAL { ?person <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs } .
	}
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(instance, id));

            return BaseController.Execute(query);
        }
    }
}
