namespace Contentful
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ClassAttribute : Attribute
    {
        public ClassAttribute(string uri)
        {
            this.Uri = uri;
        }

        public string Uri { get; private set; }
    }
}
