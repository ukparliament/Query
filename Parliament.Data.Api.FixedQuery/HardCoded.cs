namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    // TODO: Eliminate
    public class HardCoded : BaseController
    {
        public static object constituency_lookup_by_postcode(string postcode)
        {
            var externalQueryString = Resources.GetSparql("constituency_lookup_by_postcode-external");
            var queryString = Resources.GetSparql("constituency_lookup_by_postcode");

            postcode = postcode.ToUpperInvariant().Replace(" ", string.Empty);

            GetCoordinates(postcode, externalQueryString, out string latitude, out string longitude);

            var query = new SparqlParameterizedString(queryString);

            query.SetUri("schemaUri", Schema);
            query.SetLiteral("longitude", longitude);
            query.SetLiteral("latitude", latitude);

            return BaseController.ExecuteSingle(query);
        }

        public static IGraph webarticle_by_id(string webarticle_id)
        {
            var uri = new Uri(BaseController.Instance, webarticle_id).AbsoluteUri;
            var articleExists = FixedQueryController.ExecuteNamedSparql("exists", new Dictionary<string, string> { { "uri", uri } }) as SparqlResultSet;
            if (!articleExists.Result)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            try
            {
                return Contentful.Engine.GetArticle(webarticle_id);
            }
            catch (Contentful.EntryNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        public static IGraph concept_by_id(string concept_id)
        {
            //var uri = new Uri(BaseController.Instance, topic_id).AbsoluteUri;
           // var articleExists = FixedQueryController.ExecuteNamedSparql("exists", new Dictionary<string, string> { { "uri", uri } }) as SparqlResultSet;
            //if (!articleExists.Result)
            //{
             //   throw new HttpResponseException(HttpStatusCode.NotFound);
            //}

            try
            {
                return Contentful.Engine.GetConcept(concept_id);
            }
            catch (Contentful.EntryNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        public static IGraph concept_index()
        {
            //var uri = new Uri(BaseController.Instance, topic_id).AbsoluteUri;
            // var articleExists = FixedQueryController.ExecuteNamedSparql("exists", new Dictionary<string, string> { { "uri", uri } }) as SparqlResultSet;
            //if (!articleExists.Result)
            //{
            //   throw new HttpResponseException(HttpStatusCode.NotFound);
            //}

            try
            {
                return Contentful.Engine.GetConcepts();
            }
            catch (Contentful.EntryNotFoundException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
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
                var externalResults = BaseController.ExecuteList(externalQuery, "http://data.ordnancesurvey.co.uk/datasets/os-linked-data/apis/sparql") as IGraph;

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
