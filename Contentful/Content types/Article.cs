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
    using System.Collections.Generic;

    [Class("https://id.parliament.uk/schema/WebArticle")]
    [BaseUri("https://id.parliament.uk/")]
    public class Article : ContentfulClass, IUriEntry
    {
        public static string ContentTypeName => "article";

        [Predicate("http://example.com/content/schema/title")]
        public string Title { get; set; }

        [Predicate("http://example.com/content/schema/summary")]
        public string Summary { get; set; }

        [Predicate("http://example.com/content/schema/body")]
        public string Body { get; set; }

        [Predicate("http://example.com/content/schema/relatedArticle")]
        public IEnumerable<Article> RelatedArticle { get; set; }

        [Predicate("https://id.parliament.uk/schema/subjectTaggedThingHasConcept")]
        public IEnumerable<Concept> Topic { get; set; }

        [Predicate("http://example.com/content/schema/articleType")]
        public IEnumerable<ArticleType> ArticleType { get; set; }

        [Predicate("http://example.com/content/schema/audience")]
        public IEnumerable<Audience> Audience { get; set; }

        [Predicate("http://example.com/content/schema/publisher")]
        public IEnumerable<Publisher> Publisher { get; set; }

        [Predicate("http://example.com/content/schema/articleHasCollection")]
        public IEnumerable<Collection> Collections { get; set; }

        public string ParliamentId { get; set; }

        public string Uri => this.ParliamentId;
    }
}
