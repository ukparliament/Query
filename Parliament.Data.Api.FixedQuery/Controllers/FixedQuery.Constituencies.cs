namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class FixedQueryController
    {
        [HttpGet]
        public Graph constituency_by_id(string constituency_id)
        {
            var queryString = base.GetSparql("constituency_by_id");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, constituency_id));

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph constituency_map(string constituency_id)
        {
            var queryString = base.GetSparql("constituency_map");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("id", new Uri(BaseController.instance, constituency_id));

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph constituency_by_initial(string initial)
        {
            var queryString = base.GetSparql("constituency_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph constituency_current()
        {
            var queryString = base.GetSparql("constituency_current");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph constituency_lookup(string property, string value) => base.LookupInternal("ConstituencyGroup", property, value);

        [HttpGet]
        public Graph constituency_by_substring(string substring)
        {
            var queryString = base.GetSparql("constituency_by_substring");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("substring", substring);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph constituency_a_to_z()
        {
            var queryString = base.GetSparql("constituency_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph constituency_current_by_initial(string initial)
        {
            var queryString = base.GetSparql("constituency_current_by_initial");

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("initial", initial);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph constituency_current_a_to_z()
        {
            var queryString = base.GetSparql("constituency_current_a_to_z");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph constituency_index()
        {
            var queryString = base.GetSparql("constituency_index");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph constituency_members(string constituency_id)
        {
            var queryString = base.GetSparql("constituency_members");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("constituencyid", new Uri(BaseController.instance, constituency_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph constituency_current_member(string constituency_id)
        {
            var queryString = base.GetSparql("constituency_current_member");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("constituencyid", new Uri(BaseController.instance, constituency_id));

            return BaseController.ExecuteList(query);
        }

        [HttpGet]
        public Graph find_your_constituency()
        {
            var queryString = base.GetSparql("find_your_constituency");

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteSingle(query, "http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql");
        }

        // TODO: Why is this singular? - still not completely solved here
        [HttpGet]
        public Graph constituency_contact_point(string constituency_id)
        {
            var queryString = base.GetSparql("constituency_contact_point");

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("constituencyid", new Uri(BaseController.instance, constituency_id));

            return BaseController.ExecuteSingle(query);
        }

        [HttpGet]
        public Graph constituency_lookup_by_postcode(string postcode)
        {
            var externalQueryString = base.GetSparql("constituency_lookup_by_postcode-external");
            var queryString = base.GetSparql("constituency_lookup_by_postcode");

            postcode = postcode.ToUpperInvariant().Replace(" ", string.Empty);

            GetCoordinates(postcode, externalQueryString, out string latitude, out string longitude);

            var query = new SparqlParameterizedString(queryString);

            query.SetLiteral("longitude", longitude);
            query.SetLiteral("latitude", latitude);

            return BaseController.ExecuteSingle(query);
        }

        private static void GetCoordinates(string postcode, string externalQueryString, out string latitude, out string longitude)
        {
            if (postcode.StartsWith("BT"))
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        var json = client.GetStringAsync($"http://api.postcodes.io/postcodes/{postcode}").Result;

                        dynamic postcodesioJson = JsonConvert.DeserializeObject(json);

                        latitude = postcodesioJson.result.latitude;
                        longitude = postcodesioJson.result.longitude;
                    }
                    catch (AggregateException e) when (e.InnerException is HttpRequestException)
                    {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                }
            }
            else
            {
                var externalQuery = new SparqlParameterizedString(externalQueryString);
                externalQuery.SetUri("postcode", new Uri(new Uri("http://data.ordnancesurvey.co.uk/id/postcodeunit/"), postcode));
                var externalResults = BaseController.ExecuteList(externalQuery, "http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql");

                if (externalResults.Triples.Any())
                {
                    var longitudeObject = (LiteralNode)externalResults.GetTriplesWithPredicate(new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#long")).SingleOrDefault().Object;
                    var latitudeObject = (LiteralNode)externalResults.GetTriplesWithPredicate(new Uri("http://www.w3.org/2003/01/geo/wgs84_pos#lat")).SingleOrDefault().Object;

                    longitude = longitudeObject.Value;
                    latitude = latitudeObject.Value;
                }
                else
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
            }
        }
    }
}
