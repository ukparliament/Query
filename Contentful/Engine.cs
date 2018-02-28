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
            var articleQuery = QueryBuilder<Article>.New.ContentTypeIs(Article.ContentTypeName).FieldMatches(e => e.ParliamentId, id);
            var articles = Engine.client.GetEntries(articleQuery).Result;

            if (!articles.Any())
            {
                throw new EntryNotFoundException();
            }

            var article = articles.Single();

            if (article.RelatedArticle != null)
            {
                foreach (var relatedArticle in article.RelatedArticle)
                {
                    relatedArticle.Body = null;
                    relatedArticle.Summary = null;
                    relatedArticle.RelatedArticle = null;
                    relatedArticle.Topic = null;
                }
            }

            if (article.Topic != null)
            {
                foreach (var concept in article.Topic)
                {
                    concept.Definition = null;
                    concept.Description = null;
                }
            }

            var collectionQuery = QueryBuilder<Collection>.New.ContentTypeIs(Collection.ContentTypeName).LinksToEntry(article.Sys.Id);
            article.Collections = Engine.client.GetEntries(collectionQuery).Result;

            foreach (var collection in article.Collections)
            {
                collection.Articles = null;
                collection.Subcollections = null;
            }

            return new Processor(article).Graph;
        }

        public static IGraph GetConcept(string id)
        {
            var conceptQuery = QueryBuilder<Concept>.New.ContentTypeIs(Concept.ContentTypeName).FieldMatches(t => t.ParliamentId, id);
            var concepts = Engine.client.GetEntries(conceptQuery).Result;

            if (!concepts.Any())
            {
                throw new EntryNotFoundException();
            }

            var concept = concepts.Single();

            concept.Description = null;
            if (concept.BroaderConcept != null)
            {
                foreach (var broaderConcept in concept.BroaderConcept)
                {
                    broaderConcept.Description = null;
                }
            }

            var indexedArticleQuery = QueryBuilder<Article>.New.ContentTypeIs(Article.ContentTypeName).LinksToEntry(concept.Sys.Id);
            concept.IndexedArticles = client.GetEntries(indexedArticleQuery).Result;

            foreach (var indexedArticle in concept.IndexedArticles)
            {
                indexedArticle.Body = null;
                indexedArticle.Summary = null;
                indexedArticle.RelatedArticle = null;
                indexedArticle.Topic = null;
                indexedArticle.ArticleType = null;
            }

            var narrowerConceptQuery = QueryBuilder<Concept>.New.ContentTypeIs(Concept.ContentTypeName).LinksToEntry(concept.Sys.Id);
            concept.NarrowerConcepts = client.GetEntries(narrowerConceptQuery).Result;

            foreach (var narrowerConcept in concept.NarrowerConcepts)
            {
                narrowerConcept.BroaderConcept = null;
                narrowerConcept.Definition = null;
                narrowerConcept.Description = null;
            }

            return new Processor(concept).Graph;
        }

        public static IGraph GetConcepts()
        {
            var conceptQuery = QueryBuilder<Concept>.New.ContentTypeIs(Concept.ContentTypeName).Limit(1000);
            var concepts = Engine.client.GetEntries(conceptQuery).Result;

            foreach (var concept in concepts)
            {
                concept.BroaderConcept = null;
            }

            return new Processor(concepts).Graph;
        }

        public static IGraph GetCollections()
        {
            var collectionQuery = QueryBuilder<Collection>.New.ContentTypeIs(Collection.ContentTypeName);
            var collections = Engine.client.GetEntries(collectionQuery).Result;

            foreach (var collection in collections)
            {
                collection.Articles = null;
                collection.Subcollections = null;
            }

            return new Processor(collections).Graph;
        }

        public static IGraph GetCollection(string id)
        {
            var collectionQuery = QueryBuilder<Collection>.New.ContentTypeIs(Collection.ContentTypeName).FieldMatches(t => t.ParliamentId, id);
            var collections = Engine.client.GetEntries(collectionQuery).Result;

            if (!collections.Any())
            {
                throw new EntryNotFoundException();
            }

            var collection = collections.Single();

            if (collection.Subcollections != null)
            {
                foreach (var subCollection in collection.Subcollections)
                {
                    subCollection.Description = null;
                    subCollection.Articles = null;
                    subCollection.Subcollections = null;
                }
            }

            if (collection.Articles != null)
            {
                foreach (var article in collection.Articles)
                {
                    article.Body = null;
                    article.Summary = null;
                    article.RelatedArticle = null;
                    article.Topic = null;
                    article.ArticleType = null;
                }
            }

            var parentQuery = QueryBuilder<Collection>.New.ContentTypeIs(Collection.ContentTypeName).LinksToEntry(collection.Sys.Id);
            collection.Parents = Engine.client.GetEntries(parentQuery).Result;

            foreach (var parent in collection.Parents)
            {
                parent.Description = null;
                parent.Articles = null;
                parent.Subcollections = null;
            }

            return new Processor(collections).Graph;
        }
    }
}