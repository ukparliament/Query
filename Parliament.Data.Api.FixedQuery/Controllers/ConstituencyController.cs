namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("constituencies")]
    public class ConstituencyController : BaseController
    {
        // Ruby route: match '/constituencies/:constituency', to: 'constituencies#show', constituency: /\w{8}-\w{4}-\w{4}-\w{4}-\w{12}/, via: [:get]
        [Route("{id:guid}", Name = "ConstituencyByID")]
        [HttpGet]
        public Graph ById(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupEndDate ?endDate ;
        :constituencyGroupStartDate ?startDate ;
        :constituencyGroupName ?name ;
        :constituencyGroupOnsCode ?onsCode ;
        :constituencyGroupHasConstituencyArea ?constituencyArea ;
        :constituencyGroupHasHouseSeat ?houseSeat .
    ?constituencyArea
        a :ConstituencyArea ;
        :constituencyAreaLatitude ?latitude ;
        :constituencyAreaLongitude ?longitude ;
        :constituencyAreaExtent ?polygon .
    ?houseSeat
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency
        a :SeatIncumbency ;
        :incumbencyHasMember ?member ;
        :incumbencyEndDate ?incumbencyEndDate ;
        :incumbencyStartDate ?incumbencyStartDate .
    ?member
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName .
}
WHERE {
    BIND (@id AS ?constituencyGroup)

    ?constituencyGroup a :ConstituencyGroup .
    OPTIONAL { ?constituencyGroup :constituencyGroupEndDate ?endDate . }
    OPTIONAL { ?constituencyGroup :constituencyGroupStartDate ?startDate . }
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
    OPTIONAL { ?constituencyGroup :constituencyGroupOnsCode ?onsCode . }
    OPTIONAL {
        ?constituencyGroup :constituencyGroupHasConstituencyArea ?constituencyArea .
        ?constituencyArea a :ConstituencyArea .
        OPTIONAL { ?constituencyArea :constituencyAreaLatitude ?latitude . }
        OPTIONAL { ?constituencyArea :constituencyAreaLongitude ?longitude . }
        OPTIONAL { ?constituencyArea :constituencyAreaExtent ?polygon . }
    }
    OPTIONAL {
        ?constituencyGroup :constituencyGroupHasHouseSeat ?houseSeat .
        ?houseSeat :houseSeatHasSeatIncumbency ?seatIncumbency .
        ?seatIncumbency a :SeatIncumbency .
        OPTIONAL { ?seatIncumbency :incumbencyHasMember ?member . }
        OPTIONAL { ?seatIncumbency :incumbencyEndDate ?incumbencyEndDate . }
        OPTIONAL { ?seatIncumbency :incumbencyStartDate ?incumbencyStartDate . }
        OPTIONAL { ?member :personGivenName ?givenName . }
        OPTIONAL { ?member :personFamilyName ?familyName . }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, id));

            return BaseController.Execute(query);
        }

        // Ruby route: match '/constituencies/:letter', to: 'constituencies#letters', letter: /[A-Za-z]/, via: [:get]
        [Route("{initial:alpha:maxlength(1)}", Name = "ConstituencyByInitial")]
        [HttpGet]
        public Graph ByInitial(string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name .
}
WHERE {
    ?constituencyGroup a :ConstituencyGroup .
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }

    FILTER STRSTARTS(LCASE(?name), LCASE(@letter)) 
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letter", initial);

            return BaseController.Execute(query);
        }

        // Ruby route: get '/constituencies/current', to: 'constituencies#current'
        [Route("current", Name = "ConstituencyCurrent")]
        [HttpGet]
        public Graph Current()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name .
}
WHERE {
    ?constituencyGroup a :ConstituencyGroup .
    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
}
";

            return BaseController.Execute(queryString);
        }

        // Ruby route: get '/constituencies/lookup', to: 'constituencies#lookup'
        [Route("lookup/{source:alpha}/{id}", Name = "ConstituencyLookup")]
        [HttpGet]
        public Graph Lookup(string source, string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?constituency a :ConstituencyGroup .
}
WHERE {
    BIND(@id AS ?id)
    BIND(@source AS ?source)

    ?constituency
        a :ConstituencyGroup ;
        ?source ?id .
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("source", new Uri(BaseController.schema, source));
            query.SetLiteral("id", id);

            return BaseController.Execute(query);
        }

        // Ruby route: get '/constituencies/:letters', to: 'constituencies#lookup_by_letters'
        // Was this not going to be called ByInitials? - CJA

        [Route("{letters:alpha:minlength(2)}", Name = "ConstituencyByLetters")]
        [HttpGet]
        public Graph ByLetters(string letters)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?constituency
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .
}
WHERE {
    ?constituency
        a :ConstituencyGroup ;
        :constituencyGroupName ?constituencyName .

    FILTER CONTAINS(LCASE(?constituencyName), LCASE(@letters))
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("letters", letters);

            return BaseController.Execute(query);
        }

        // Ruby route: get '/constituencies/a_z_letters', to: 'constituencies#a_z_letters'
        [Route("a_z_letters", Name = "ConstituencyAToZ")]
        [HttpGet]
        public Graph AToZLetters()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
     _:x :value ?firstLetter.
}
WHERE {
    SELECT DISTINCT ?firstLetter WHERE {
    ?constituency :constituencyGroupName ?constituencyName .
    BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
    }
}
";
    
            var query = new SparqlParameterizedString(queryString);
            return BaseController.Execute(query);
        }

        // Ruby route: match '/constituencies/current/:letter', to: 'constituencies#current_letters', letter: /[A-Za-z]/, via: [:get]
        [Route("current/{initial:maxlength(1)}", Name = "ConstituencyCurrentByInitial")]
        [HttpGet]
        public Graph CurrentByLetters(string initial)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name .
}
WHERE {
    ?constituencyGroup a :ConstituencyGroup .
    FILTER NOT EXISTS { ?constituencyGroup a :PastConstituencyGroup . }
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
    FILTER STRSTARTS(LCASE(?name), LCASE(@initial))
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.Execute(query);
        }
        // Ruby route: get '/constituencies/current/a_z_letters', to: 'constituencies#a_z_letters_current'
        [Route("current/a_z_letters", Name = "ConstituencyCurrentAToZ")]
        [HttpGet]
        public Graph CurrentAToZLetters()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT {
     _:x :value ?firstLetter.
}
WHERE {
    SELECT DISTINCT ?firstLetter WHERE {
    ?constituency :constituencyGroupName ?constituencyName.
    FILTER NOT EXISTS {?constituency a :PastConstituencyGroup. }

    BIND(ucase(SUBSTR(?constituencyName, 1, 1)) as ?firstLetter)
    }
}
";

            var query = new SparqlParameterizedString(queryString);
            return BaseController.Execute(query);
        }

        // Ruby route: resources :constituencies, only: [:index]
        [Route("", Name = "ConstituencyIndex")]
        [HttpGet]
        public Graph Index()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT{
    ?constituencyGroup
        a :ConstituencyGroup ;
        :constituencyGroupName ?name .
    }
WHERE {
 	?constituencyGroup a :ConstituencyGroup .
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
}
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.Execute(query);
        }

        // Ruby route: resources :constituencies, only: [:index] do get '/members', to: 'constituencies#members' end
        [Route("{id:guid}/members", Name = "ConstituencyMembers")]
        [HttpGet]
        public Graph Members(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>

CONSTRUCT{
   	?constituencyGroup
        a :ConstituencyGroup ;
   		:constituencyGroupName ?name ;
   		:constituencyGroupHasHouseSeat ?houseSeat ;
        :constituencyGroupStartDate ?constituencyGroupStartDate ;
        :constituencyGroupEndDate ?constituencyGroupEndDate .
   	?houseSeat 
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency 
        a :SeatIncumbency ;
        :incumbencyHasMember ?member ;
        :incumbencyEndDate ?seatIncumbencyEndDate ;
        :incumbencyStartDate ?seatIncumbencyStartDate .
    ?member 
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName .
    }
WHERE {
    BIND( @constituencyid AS ?constituencyGroup )
    ?constituencyGroup :constituencyGroupHasHouseSeat ?houseSeat .
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
    OPTIONAL { ?constituencyGroup :constituencyGroupEndDate ?constituencyGroupEndDate . }
    OPTIONAL { ?constituencyGroup :constituencyGroupStartDate ?constituencyGroupStartDate . }
    OPTIONAL {
        ?houseSeat :houseSeatHasSeatIncumbency ?seatIncumbency .
        OPTIONAL {
            ?seatIncumbency :incumbencyHasMember ?member .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        	OPTIONAL { ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate . }
        	OPTIONAL { ?member :personGivenName ?givenName . }
        	OPTIONAL { ?member :personFamilyName ?familyName . }
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("constituencyid", new Uri(BaseController.instance, id));

            return BaseController.Execute(query);
        }

        // Ruby route: resources :constituencies, only: [:index] do get '/members/current', to: 'constituencies#current_member' end
        [Route("{id:guid}/members/current", Name = "ConstituencyCurrentMembers")]
        [HttpGet]
        public Graph CurrentMembers(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     
CONSTRUCT{
   	?constituencyGroup
       	a :ConstituencyGroup ;
		:constituencyGroupName ?name ;
        :constituencyGroupStartDate ?constituencyGroupStartDate ;
        :constituencyGroupHasHouseSeat ?houseSeat .
    ?houseSeat 
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?seatIncumbency .
    ?seatIncumbency 
        a :SeatIncumbency ;
        :incumbencyHasMember ?member ;
        :incumbencyEndDate ?seatIncumbencyEndDate ;
        :incumbencyStartDate ?seatIncumbencyStartDate .
    ?member 
        a :Person ;
        :personGivenName ?givenName ;
        :personFamilyName ?familyName .
    }
WHERE {
    BIND( @constituencyid AS ?constituencyGroup )
  	?constituencyGroup :constituencyGroupHasHouseSeat ?houseSeat .
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
    OPTIONAL { ?constituencyGroup :constituencyGroupStartDate ?constituencyGroupStartDate . }
    OPTIONAL {
        ?houseSeat :houseSeatHasSeatIncumbency ?seatIncumbency .
        FILTER NOT EXISTS { ?seatIncumbency a :PastIncumbency . }
        OPTIONAL {
    	    ?seatIncumbency :incumbencyHasMember ?member .
            OPTIONAL { ?seatIncumbency :incumbencyEndDate ?seatIncumbencyEndDate . }
        	OPTIONAL { ?seatIncumbency :incumbencyStartDate ?seatIncumbencyStartDate . }
        	OPTIONAL { ?member :personGivenName ?givenName . }
        	OPTIONAL { ?member :personFamilyName ?familyName . }
        }
    }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("constituencyid", new Uri(BaseController.instance, id));

            return BaseController.Execute(query);
        }

        // Ruby route: resources :constituencies, only: [:index] do get '/contact_point', to: 'constituencies#contact_point' end
        // why is this singular?
        [Route("{id:guid}/contact_point", Name = "ConstituencyContactPoint")]
        [HttpGet]
        public Graph ContactPoint(string id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
     
CONSTRUCT {
   	?constituencyGroup 
        a :ConstituencyGroup ;
        :constituencyGroupHasHouseSeat ?houseSeat ;
        :constituencyGroupName ?name .
    ?houseSeat 
        a :HouseSeat ;
        :houseSeatHasSeatIncumbency ?incumbency .
    ?incumbency 
        a :SeatIncumbency ;
        :incumbencyHasContactPoint ?contactPoint .
    ?contactPoint 
        a :ContactPoint ;
        :email ?email ;
        :phoneNumber ?phoneNumber ;
        :faxNumber ?faxNumber ;
    	:contactForm ?contactForm ;
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
    BIND( @constituencyid AS ?constituencyGroup )
    OPTIONAL {
       	    ?constituencyGroup :constituencyGroupHasHouseSeat ?houseSeat .
        	OPTIONAL {
        		?houseSeat :houseSeatHasSeatIncumbency ?incumbency .
        		FILTER NOT EXISTS { ?incumbency a :PastIncumbency . }
        		OPTIONAL {
            		?incumbency :incumbencyHasContactPoint ?contactPoint .
                    OPTIONAL{ ?contactPoint :email ?email . }
                    OPTIONAL{ ?contactPoint :phoneNumber ?phoneNumber . }
                    OPTIONAL{ ?contactPoint :faxNumber ?faxNumber . }
                    OPTIONAL{ ?contactPoint :contactForm ?contactForm . }
                    OPTIONAL{ ?contactPoint :contactPointHasPostalAddress ?postalAddress .
                        OPTIONAL{ ?postalAddress :postCode ?postCode . }
                        OPTIONAL{ ?postalAddress :addressLine1 ?addressLine1 . }
                        OPTIONAL{ ?postalAddress :addressLine2 ?addressLine2 . }
                        OPTIONAL{ ?postalAddress :addressLine3 ?addressLine3 . }
                        OPTIONAL{ ?postalAddress :addressLine4 ?addressLine4 . }
                        OPTIONAL{ ?postalAddress :addressLine5 ?addressLine5 . }
                    }
                }
        	}
    	}
    OPTIONAL { ?constituencyGroup :constituencyGroupName ?name . }
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("constituencyid", new Uri(BaseController.instance, id));

            return BaseController.Execute(query);
        }

    }
}