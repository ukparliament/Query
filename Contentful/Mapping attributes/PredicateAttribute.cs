namespace Contentful
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PredicateAttribute : Attribute
    {
        public PredicateAttribute(string uri)
        {
            this.Uri = uri;
        }

        public string Uri { get; private set; }
    }
}
