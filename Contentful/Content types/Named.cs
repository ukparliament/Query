namespace Contentful
{
    [BaseUri("http://example.com/content/")]
    public abstract class Named : ContentfulClass
    {
        [Predicate("http://example.com/content/schema/name")]
        public string Name { get; set; }
    }
}
