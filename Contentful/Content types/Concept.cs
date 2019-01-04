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

    [Class("https://id.parliament.uk/schema/Concept")]
    [BaseUri("https://id.parliament.uk/")]
    public class Concept : ContentfulClass, IUriEntry
    {
        public static string ContentTypeName => "topic";

        [Predicate("https://id.parliament.uk/schema/conceptLabel")]
        public string Name { get; set; }

        [Predicate("https://id.parliament.uk/schema/conceptDefinition")]
        public string Definition { get; set; }

        [Predicate("https://id.parliament.uk/schema/conceptScopeNote")]
        public string Description { get; set; }

        [Predicate("https://id.parliament.uk/schema/conceptHasBroaderConcept")]
        public IEnumerable<Concept> BroaderConcept { get; set; }

        public string ParliamentId { get; set; }

        public string Uri => this.ParliamentId;

        [Predicate("https://id.parliament.uk/schema/conceptHasSubjectTaggedThing")]
        public IEnumerable<Article> IndexedArticles { get; set; }

        [Predicate("https://id.parliament.uk/schema/conceptHasNarrowerConcept")]
        public IEnumerable<Concept> NarrowerConcepts { get; set; }
    }
}
