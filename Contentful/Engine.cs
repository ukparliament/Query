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
        private static readonly ContentfulClient client = new ContentfulClient(
            new HttpClient(),
            new ContentfulOptions
            {
                DeliveryApiKey = ConfigurationManager.AppSettings["ContentfulDeliveryApiKey"],
                SpaceId = ConfigurationManager.AppSettings["ContentfulSpaceId"]
            }
        );

        public static IGraph GetArticle(string id)
        {
            var builder = QueryBuilder<Article>.New.ContentTypeIs("article").FieldMatches(e => e.ParliamentId, id);
            var entries = Engine.client.GetEntries(builder).Result;

            if (!entries.Any())
            {
                throw new EntryNotFoundException();
            }

            return new Processor(entries.Single()).Graph;
        }

        public static IGraph GetConcept(string id)
        {
            var conceptBuilder = QueryBuilder<Concept>.New.ContentTypeIs(Concept.ContentTypeName).FieldMatches(t => t.ParliamentId, id);
            var concepts = Engine.client.GetEntries(conceptBuilder).Result;
            var concept = concepts.Single();
            var conceptId = concept.Sys.Id;

            var indexedArticleBuilder = QueryBuilder<Article>.New.ContentTypeIs(Article.ContentTypeName).LinksToEntry(conceptId).Include(10);
            var indexedArticles = client.GetEntries(indexedArticleBuilder).Result;
            concept.IndexedArticles = indexedArticles;

            var narrowerConceptBuilder = QueryBuilder<Concept>.New.ContentTypeIs(Concept.ContentTypeName).LinksToEntry(conceptId).Include(10);
            var narrowerConcepts = client.GetEntries(narrowerConceptBuilder).Result;
            concept.NarrowerConcepts = narrowerConcepts;

            if (!concepts.Any())
            {
                throw new EntryNotFoundException();
            }

            return new Processor(concept).Graph;
        }

        public static IGraph GetConcepts()
        {
            var conceptBuilder = QueryBuilder<Concept>.New.ContentTypeIs(Concept.ContentTypeName);
            var concepts = Engine.client.GetEntries(conceptBuilder).Result;

            return new Processor(concepts).Graph;
        }
    }
}