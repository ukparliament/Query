// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Contentful
{
    using Contentful.Core;
    using Contentful.Core.Configuration;
    using Contentful.Core.Search;
    using System.Collections.Generic;
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

            foreach (var relatedArticle in article.RelatedArticle ?? Enumerable.Empty<Article>())
            {
                relatedArticle.Body = null;
                relatedArticle.Collections = null;
                relatedArticle.RelatedArticle = null;
                relatedArticle.Topic = null;
            }

            foreach (var concept in article.Topic ?? Enumerable.Empty<Concept>())
            {
                concept.Definition = null;
                concept.Description = null;
            }

            var collectionQuery = QueryBuilder<Collection>.New.ContentTypeIs(Collection.ContentTypeName).LinksToEntry(article.Sys.Id);
            article.Collections = Engine.client.GetEntries(collectionQuery).Result;

            foreach (var collection in article.Collections)
            {
                AddParents(collection);

                collection.ExtendedDescription = null;
                collection.Subcollections = null;

                foreach (var siblingArticle in collection.Articles ?? Enumerable.Empty<Article>())
                {
                    siblingArticle.Body = null;
                    siblingArticle.Collections = null;
                    siblingArticle.RelatedArticle = null;
                    siblingArticle.Topic = null;
                }
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

            foreach (var broaderConcept in concept.BroaderConcept ?? Enumerable.Empty<Concept>())
            {
                broaderConcept.Description = null;
            }

            var indexedArticleQuery = QueryBuilder<Article>.New.ContentTypeIs(Article.ContentTypeName).LinksToEntry(concept.Sys.Id);
            concept.IndexedArticles = client.GetEntries(indexedArticleQuery).Result;

            foreach (var indexedArticle in concept.IndexedArticles)
            {
                indexedArticle.Body = null;
                indexedArticle.RelatedArticle = null;
                indexedArticle.Topic = null;
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

                foreach (var subCollection in collection.Subcollections ?? Enumerable.Empty<Collection>())
                {
                    subCollection.Articles = null;
                    subCollection.ExtendedDescription = null;
                }
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

            foreach (var subCollection in collection.Subcollections ?? Enumerable.Empty<Collection>())
            {
                subCollection.Articles = null;
                subCollection.ExtendedDescription = null;
                subCollection.Subcollections = null;
            }

            foreach (var article in collection.Articles ?? Enumerable.Empty<Article>())
            {
                article.Body = null;
                article.RelatedArticle = null;
                article.Topic = null;
            }

            AddParents(collection);

            return new Processor(collections).Graph;
        }

        private static void AddParents(Collection collection)
        {
            var parentQuery = QueryBuilder<Collection>.New.ContentTypeIs(Collection.ContentTypeName).LinksToEntry(collection.Sys.Id);
            collection.Parents = Engine.client.GetEntries(parentQuery).Result;

            foreach (var parent in collection.Parents)
            {
                parent.Articles = null;
                parent.ExtendedDescription = null;
                parent.Subcollections = null;

                AddParents(parent);
            }
        }
    }
}