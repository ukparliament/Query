namespace Contentful
{
    using System.Collections.Generic;

    [Class("http://example.com/content/schema/Collection")]
    [BaseUri("https://id.parliament.uk/")]
    public class Collection : ContentfulClass, IUriEntry
    {
        public static string ContentTypeName => "collection";

        [Predicate("http://example.com/content/schema/collectionName")]
        public string Name { get; set; }

        [Predicate("http://example.com/content/schema/collectionDescription")]
        public string Description { get; set; }

        [Predicate("http://example.com/content/schema/collectionHasSubcollection")]
        public IEnumerable<Collection> Subcollections { get; set; }

        [Predicate("http://example.com/content/schema/collectionHasParent")]
        public IEnumerable<Collection> Parents { get; set; }

        [Predicate("http://example.com/content/schema/collectionHasArticle")]
        public IEnumerable<Article> Articles { get; set; }

        public string ParliamentId { get; set; }

        public string Uri => this.ParliamentId;
    }
}
