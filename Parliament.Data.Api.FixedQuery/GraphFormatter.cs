namespace Parliament.Data.Api.FixedQuery
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using VDS.RDF;
    using VDS.RDF.Writing;

    public class GraphFormatter : BufferedMediaTypeFormatter
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

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            var mapping = this.MediaTypeMappings.Single();
            var rdfWriter = GetWriter(mapping);
            var streamWriter = new StreamWriter(writeStream);
            var graph = value as IGraph;

            rdfWriter.Save(graph, streamWriter);
        }

        private static IRdfWriter GetWriter(MediaTypeMapping mapping)
        {
            var writer = null as IRdfWriter;
            var extensionMapping = mapping as UriPathExtensionMapping;
            if (extensionMapping != null)
            {
                writer = MimeTypesHelper.GetWriterByFileExtension(extensionMapping.UriPathExtension);
            }
            else
            {
                writer = MimeTypesHelper.GetWriter(mapping.MediaType.MediaType);
            }

            var htmlWriter = writer as HtmlWriter;
            if (htmlWriter != null)
            {
                htmlWriter.UriPrefix = "resource_by_id?resource_id=";
            }

            return writer;
        }
    }
}
