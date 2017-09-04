namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph party_index()
        {
            var queryString = base.GetSparql("party_index");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_by_id(string party_id)
        {
            var queryString = base.GetSparql("party_by_id");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteSingle(query);

        }

        [HttpGet]
        public Graph party_by_initial(string initial)
        {
            var queryString = base.GetSparql("party_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_current()
        {
            var queryString = base.GetSparql("party_current");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_a_to_z()
        {
            var queryString = base.GetSparql("party_a_to_z");

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_current_a_to_z()
        {
            var queryString = base.GetSparql("party_current_a_to_z");

            var query = new SparqlParameterizedString(queryString);
            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_lookup(string property, string value) => base.LookupInternal("Party", property, value);

        [HttpGet]
        public Graph party_by_substring(string substring)
        {
            var queryString = base.GetSparql("party_by_substring");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("substring", substring);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_members(string party_id)
        {
            var queryString = base.GetSparql("party_members");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_current_members(string party_id)
        {
            var queryString = base.GetSparql("party_current_members");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_members_by_initial(string party_id, string initial)
        {
            var queryString = base.GetSparql("party_members_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, party_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_members_a_to_z(string party_id)
        {
            var queryString = base.GetSparql("party_members_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_current_members_by_initial(string party_id, string initial)
        {
            var queryString = base.GetSparql("party_current_members_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, party_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph party_current_members_a_to_z(string party_id)
        {
            var queryString = base.GetSparql("party_current_members_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }
    }
}
