namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph parliament_index()
        {
            var queryString = base.GetSparql("parliament_index");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_current()
        {
            var queryString = base.GetSparql("parliament_current");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph parliament_previous()
        {
            var queryString = base.GetSparql("parliament_previous");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_next()
        {
            var queryString = base.GetSparql("parliament_next");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_lookup(string property, string value) => base.LookupInternal("ParliamentPeriod", property, value);

        [HttpGet]
        public Graph parliament_by_id(string parliament_id)
        {
            var queryString = base.GetSparql("parliament_by_id");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteSingle(query);

        }

        [HttpGet]
        public Graph next_parliament_by_id(string parliament_id)
        {
            var queryString = base.GetSparql("next_parliament_by_id");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteSingle(query);

        }

        [HttpGet]
        public Graph previous_parliament_by_id(string parliament_id)
        {
            var queryString = base.GetSparql("previous_parliament_by_id");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteSingle(query);

        }

        [HttpGet]
        public Graph parliament_members(string parliament_id)
        {
            var queryString = base.GetSparql("parliament_members");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_members_by_initial(string parliament_id, string initial)
        {
            var queryString = base.GetSparql("parliament_members_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_members_a_to_z(string parliament_id)
        {
            var queryString = base.GetSparql("parliament_members_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_houses(string parliament_id)
        {
            var queryString = base.GetSparql("parliament_houses");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_house(string parliament_id, string house_id)
        {
            var queryString = base.GetSparql("parliament_house");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph parliament_house_members(string parliament_id, string house_id)
        {
            var queryString = base.GetSparql("parliament_house_members");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_house_members_a_to_z(string parliament_id, string house_id)
        {
            var queryString = base.GetSparql("parliament_house_members_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_house_members_by_initial(string parliament_id, string house_id, string initial)
        {
            var queryString = base.GetSparql("parliament_house_members_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_parties(string parliament_id)
        {
            var queryString = base.GetSparql("parliament_parties");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_party(string parliament_id, string party_id)
        {
            var queryString = base.GetSparql("parliament_party");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_party_members(string parliament_id, string party_id)
        {
            var queryString = base.GetSparql("parliament_party_members");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_party_members_a_to_z(string parliament_id, string party_id)
        {
            var queryString = base.GetSparql("parliament_party_members_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_party_members_by_initial(string parliament_id, string party_id, string initial)
        {
            var queryString = base.GetSparql("parliament_party_members_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_house_parties(string parliament_id, string house_id)
        {
            var queryString = base.GetSparql("parliament_house_parties");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_house_party(string parliament_id, string house_id, string party_id)
        {
            var queryString = base.GetSparql("parliament_house_party");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph parliament_house_party_members(string parliament_id, string house_id, string party_id)
        {
            var queryString = base.GetSparql("parliament_house_party_members");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_house_party_members_a_to_z(string parliament_id, string house_id, string party_id)
        {
            var queryString = base.GetSparql("parliament_house_party_members_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_house_party_members_by_initial(string parliament_id, string house_id, string party_id, string initial)
        {
            var queryString = base.GetSparql("parliament_house_party_members_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetUri("houseid", new Uri(BaseController.instance, house_id));
            query.SetUri("partyid", new Uri(BaseController.instance, party_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_constituencies(string parliament_id)
        {
            var queryString = base.GetSparql("parliament_constituencies");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_constituencies_a_to_z(string parliament_id)
        {
            var queryString = base.GetSparql("parliament_constituencies_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph parliament_constituencies_by_initial(string parliament_id, string initial)
        {
            var queryString = base.GetSparql("parliament_constituencies_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("parliamentid", new Uri(BaseController.instance, parliament_id));
            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }
    }
}
