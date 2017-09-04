namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        //[Route("", Name = "FormalBodyIndex")]
        [HttpGet]
        public Graph formal_body_index()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?formalBody
        a :FormalBody ;
        :formalBodyName ?formalBodyName .
}
WHERE {
    ?formalBody
        a :FormalBody ;
        :formalBodyName ?formalBodyName .
}
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        //[Route(@"{id:regex(^\w{8}$)}", Name = "FormalBodyById")]
        [HttpGet]
        public Graph formal_body_by_id(string formal_body_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?formalBody
        a :FormalBody ;
        :formalBodyName ?formalBodyName .
}
WHERE {
    BIND(@id AS ?formalBody)
    ?formalBody
        a :FormalBody ;
        :formalBodyName ?formalBodyName .
}
";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, formal_body_id));

            return BaseController.ExecuteSingle(query);

        }

        //[Route(@"{id:regex(^\w{8}$)}/members", Name = "FormalBodyMembership")]
        [HttpGet]
        public Graph formal_body_membership(string formal_body_id)
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?formalBody a :FormalBody ;
        :formalBodyHasFormalBodyMembership ?membership .
    ?membership a :FormalBodyMembership ;
        :formalBodyMembershipHasPerson ?person .
}
WHERE {
    BIND(@id AS ?formalBody)
    ?formalBody a :FormalBody ;
        :formalBodyHasFormalBodyMembership ?membership .
    ?membership :formalBodyMembershipHasPerson ?person .
}";

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, formal_body_id));

            return BaseController.ExecuteList(query);
        }

    }
}
