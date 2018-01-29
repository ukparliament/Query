namespace Contentful
{
    using Contentful.Core.Models;
    using System.Collections.Generic;

    [Class("https://id.parliament.uk/schema/Concept")]
    [BaseUri("https://id.parliament.uk/")]

   
    public class Concept : ContentfulClass, IUriEntry
    {
        public static string ContentTypeName => "topic";

        [Predicate("https://id.parliament.uk/schema/conceptLabel")]
        public string Name { get; set; }

        [Predicate("https://id.parliament.uk/schema/Concept")]
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
