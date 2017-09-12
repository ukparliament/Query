namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph person_index()
        {
            var queryString = base.GetSparql("person_index");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph person_by_id(string person_id)
        {
            var queryString = base.GetSparql("person_by_id");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(instance, person_id));

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph person_by_initial(string initial)
        {
            var queryString = base.GetSparql("person_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);

        }

        [HttpGet]
        public Graph person_lookup(string property, string value) => base.LookupInternal("Person", property, value);

        [HttpGet]
        public Graph member_index()
        {
            var queryString = base.GetSparql("person_lookup");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph person_by_substring(string substring)
        {
            var queryString = base.GetSparql("person_by_substring");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("substring", substring);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph person_a_to_z()
        {
            var queryString = base.GetSparql("person_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph person_constituencies(string person_id)
        {
            var queryString = base.GetSparql("person_constituencies");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, person_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph person_current_constituency(string person_id)
        {
            var queryString = base.GetSparql("person_current_constituency");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, person_id));

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph person_parties(string person_id)
        {
            var queryString = base.GetSparql("person_parties");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, person_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph person_current_party(string person_id)
        {
            var queryString = base.GetSparql("person_current_party");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, person_id));

            return BaseController.ExecuteSingle(query);
        }

        // note: query currently only really returns parliamentary contact point, not "contact points"
        [HttpGet]
        public Graph person_contact_points(string person_id)
        {
            var queryString = base.GetSparql("person_contact_points");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, person_id));

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph person_houses(string person_id)
        {
            var queryString = base.GetSparql("person_houses");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, person_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph person_current_house(string person_id)
        {
            var queryString = base.GetSparql("person_current_house");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, person_id));

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph person_mps()
        {
            var queryString = base.GetSparql("person_mps");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph person_committees_index(string person_id)
        {
            var queryString = base.GetSparql("person_committees_index");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, person_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph person_committees_memberships_index(string person_id)
        {
            var queryString = base.GetSparql("person_committees_memberships_index");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, person_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph person_current_committees_memberships(string person_id)
        {
            var queryString = base.GetSparql("person_current_committees_memberships");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("personid", new Uri(BaseController.instance, person_id));

            return BaseController.ExecuteList(query);
        }
    }
}
