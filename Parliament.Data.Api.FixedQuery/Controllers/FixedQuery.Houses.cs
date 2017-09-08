namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph house_by_id(string house_id)
        {
            var queryString = base.GetSparql("house_by_id");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteSingle(query);

        }

        [HttpGet]
        public Graph house_lookup(string property, string value) => base.LookupInternal("House", property, value);

        [HttpGet]
        public Graph house_by_substring(string substring)
        {
            var queryString = base.GetSparql("house_by_substring");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("substring", substring);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_index()
        {
            var queryString = base.GetSparql("house_index");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_members(string house_id)
        {
            var queryString = base.GetSparql("house_members");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_current_members(string house_id)
        {
            var queryString = base.GetSparql("house_current_members");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_parties(string house_id)
        {
            var queryString = base.GetSparql("house_parties");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_current_parties(string house_id)
        {
            var queryString = base.GetSparql("house_current_parties");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_party_by_id(string house_id, string party_id)
        {
            var queryString = base.GetSparql("house_party_by_id");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph house_members_by_initial(string house_id, string initial)
        {
            var queryString = base.GetSparql("house_members_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_members_a_to_z(string house_id)
        {
            var queryString = base.GetSparql("house_members_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_current_members_by_initial(string house_id, string initial)
        {
            var queryString = base.GetSparql("house_current_members_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_current_members_a_to_z(string house_id)
        {
            var queryString = base.GetSparql("house_current_members_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_party_members(string house_id, string party_id)
        {
            var queryString = base.GetSparql("house_party_members");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_party_members_by_initial(string house_id, string party_id, string initial)
        {
            var queryString = base.GetSparql("house_party_members_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_party_members_a_to_z(string house_id, string party_id)
        {
            var queryString = base.GetSparql("house_party_members_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_party_current_members(string house_id, string party_id)
        {
            var queryString = base.GetSparql("house_party_current_members");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_party_current_members_by_initial(string house_id, string party_id, string initial)
        {
            var queryString = base.GetSparql("house_party_current_members_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_party_current_members_a_to_z(string house_id, string party_id)
        {
            var queryString = base.GetSparql("house_party_current_members_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_committees_index(string house_id)
        {
            var queryString = base.GetSparql("house_committees_index");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_committees_a_to_z(string house_id)
        {
            var queryString = base.GetSparql("house_committees_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_committees_by_initial(string house_id, string initial)
        {
            var queryString = base.GetSparql("house_committees_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_current_committees(string house_id)
        {
            var queryString = base.GetSparql("house_current_committees");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_current_committees_a_to_z(string house_id)
        {
            var queryString = base.GetSparql("house_current_committees_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph house_current_committees_by_initial(string house_id, string initial)
        {
            var queryString = base.GetSparql("house_current_committees_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }


    }
}
