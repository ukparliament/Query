namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
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
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
           ?contactPoint
               a parl:ContactPoint ;
               parl:email ?email ;
               parl:phoneNumber ?phoneNumber ;
               parl:faxNumber ?faxNumber ;
               parl:contactPointHasPostalAddress ?postalAddress .
           ?postalAddress a parl:PostalAddress ;
                       parl:postCode ?postCode ;
               			   parl:addressLine1 ?addressLine1 ;
               			   parl:addressLine2 ?addressLine2 ;
               			   parl:addressLine3 ?addressLine3 ;
               			   parl:addressLine4 ?addressLine4 ;
               			   parl:addressLine5 ?addressLine5 .
        }
        WHERE {
        	?contactPoint a parl:ContactPoint ;
        	OPTIONAL{ ?contactPoint parl:email ?email . }
        	OPTIONAL{ ?contactPoint parl:phoneNumber ?phoneNumber . }
        	OPTIONAL{ ?contactPoint parl:faxNumber ?faxNumber . }
        	OPTIONAL{
                ?contactPoint parl:contactPointHasPostalAddress ?postalAddress .
                OPTIONAL{ ?postalAddress parl:postCode ?postCode . }
               	OPTIONAL{ ?postalAddress parl:addressLine1 ?addressLine1 . }
               	OPTIONAL{ ?postalAddress parl:addressLine2 ?addressLine2 . }
               	OPTIONAL{ ?postalAddress parl:addressLine3 ?addressLine3 . }
               	OPTIONAL{ ?postalAddress parl:addressLine4 ?addressLine4 . }
               	OPTIONAL{ ?postalAddress parl:addressLine5 ?addressLine5 . }
          	}
      }
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.Execute(query);
        }

        // Ruby route: resources :contact_points, only: [:show]
        [Route("{id:guid}", Name = "ContactPointById")]
        [HttpGet]
        public Graph ById(string id)
        {
            var queryString = @"
PREFIX parl: <http://id.ukpds.org/schema/>
     CONSTRUCT {
           ?contactPoint
               a parl:ContactPoint ;
               parl:email ?email ;
               parl:phoneNumber ?phoneNumber ;
               parl:faxNumber ?faxNumber ;
               parl:contactPointHasPostalAddress ?postalAddress ;
    		       parl:contactPointHasIncumbency ?incumbency .
    		?incumbency
        		a parl:Incumbency ;
        		parl:incumbencyHasMember ?person .
            ?postalAddress
                a parl:PostalAddress ;
                parl:postCode ?postCode ;
                parl:addressLine1 ?addressLine1 ;
                parl:addressLine2 ?addressLine2 ;
                parl:addressLine3 ?addressLine3 ;
                parl:addressLine4 ?addressLine4 ;
                parl:addressLine5 ?addressLine5 .
    		?person
              a parl:Person ;
              parl:personGivenName ?givenName ;
        			parl:personFamilyName ?familyName ;
              <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ?displayAs .
        }
        WHERE {
    		BIND(@id AS ?contactPoint )
        	?contactPoint a parl:ContactPoint ;
        	OPTIONAL{ ?contactPoint parl:email ?email . }
        	OPTIONAL{ ?contactPoint parl:phoneNumber ?phoneNumber . }
        	OPTIONAL{ ?contactPoint parl:faxNumber ?faxNumber . }
        	OPTIONAL{
                ?contactPoint parl:contactPointHasPostalAddress ?postalAddress .
                OPTIONAL{ ?postalAddress parl:postCode ?postCode . }
               	OPTIONAL{ ?postalAddress parl:addressLine1 ?addressLine1 . }
               	OPTIONAL{ ?postalAddress parl:addressLine2 ?addressLine2 . }
               	OPTIONAL{ ?postalAddress parl:addressLine3 ?addressLine3 . }
               	OPTIONAL{ ?postalAddress parl:addressLine4 ?addressLine4 . }
               	OPTIONAL{ ?postalAddress parl:addressLine5 ?addressLine5 . }
          	}
            OPTIONAL{
				      ?contactPoint parl:contactPointHasIncumbency ?incumbency .
        		  ?incumbency parl:incumbencyHasMember ?person .
        		  OPTIONAL { ?person parl:personFamilyName ?familyName . }
        		  OPTIONAL { ?person parl:personGivenName ?givenName . }
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
