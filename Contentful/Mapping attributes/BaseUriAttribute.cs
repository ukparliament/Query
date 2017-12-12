namespace Contentful
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BaseUriAttribute : Attribute
    {
        public BaseUriAttribute(string uri)
        {
            this.Uri = uri;
        }

        public string Uri { get; private set; }
    }
}
