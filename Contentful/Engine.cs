namespace Contentful
{
    using Contentful.Core;
    using Contentful.Core.Configuration;
    using Contentful.Core.Search;
    using System.Configuration;
    using System.Linq;
    using System.Net.Http;
    using VDS.RDF;

    public static class Engine
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string ContentfulDeliveryApiKey = ConfigurationManager.AppSettings["ContentfulDeliveryApiKey"];
        private static readonly string ContentfulSpaceId = ConfigurationManager.AppSettings["ContentfulSpaceId"];

        public static IGraph GetArticle(string id)
        {
            var options = new ContentfulOptions
            {
                DeliveryApiKey = Engine.ContentfulDeliveryApiKey,
                SpaceId = Engine.ContentfulSpaceId
            };

            var client = new ContentfulClient(Engine.httpClient, options);
            var builder = QueryBuilder<Article>.New.ContentTypeIs("article").FieldMatches(e => e.ParliamentId, id);
            var entry = client.GetEntries(builder).Result;

            if (!entry.Any())
            {
                throw new EntryNotFoundException();
            }

            return new Processor(entry.Single()).Graph;
        }
    }
}
