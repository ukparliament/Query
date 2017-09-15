namespace Parliament.Data.Api.FixedQuery
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;
    using VDS.RDF;
    using VDS.RDF.Writing;

    public class GraphFormatter : MediaTypeFormatter
    {
        public GraphFormatter(MediaTypeMapping mapping)
        {
            if (mapping == null)
            {
                throw new ArgumentNullException("mapping");
            }

            this.SupportedMediaTypes.Add(mapping.MediaType);
            this.MediaTypeMappings.Add(mapping);
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(IGraph).IsAssignableFrom(type);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var mediaType = this.SupportedMediaTypes.Single().MediaType;
            var writer = GetWriter(mediaType);

            return Task.Factory.StartNew(() =>
            {
                using (var sw = new StreamWriter(writeStream))
                {
                    (value as IGraph).SaveToStream(sw, writer);
                }
            });
        }

        private static IRdfWriter GetWriter(string mediaType)
        {
            var writer = MimeTypesHelper.GetWriter(mediaType);

            if (writer is HtmlWriter)
            {
                (writer as HtmlWriter).UriPrefix = "resource_by_id?resource_id=";
            }

            return writer;
        }
    }
}
