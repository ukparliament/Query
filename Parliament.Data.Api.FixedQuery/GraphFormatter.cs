namespace Parliament.Data.Api.FixedQuery
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Writing;

    public class GraphFormatter : MediaTypeFormatter
    {
        private static readonly Dictionary<string, string> mediaTypes = new Dictionary<string, string> {
            { "csv", "text/csv" },
            { "graphviz", "text/vnd.graphviz" },
            { "html", "text/html" },
            { "n3", "text/n3" },
            { "ntriples", "application/n-triples" },
            { "rdfxml", "application/rdf+xml" },
            { "rdfjson", "application/rdf+json" },
            { "tsv", "text/tab-separated-values" },
            { "turtle", "text/turtle" }
        };

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
            return typeof(Graph).IsAssignableFrom(type);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var mediaType = this.SupportedMediaTypes.Single().MediaType;
            var writer = GetWriter(mediaType);
            
            return Task.Factory.StartNew(() =>
            {
                using (var sw = new StreamWriter(writeStream))
                {
                    (value as Graph).SaveToStream(sw, writer);
                }
            });
        }

        private static IRdfWriter GetWriter(string mediaType)
        {
            if (mediaType == GraphFormatter.mediaTypes["csv"])
            {
                return new CsvWriter();
            }

            if (mediaType == GraphFormatter.mediaTypes["graphviz"])
            {
                return new GraphVizWriter();
            }

            if (mediaType == GraphFormatter.mediaTypes["html"])
            {
                return new HtmlWriter()
                {
                    UriPrefix = "resource_by_id?uri=",
                };
            }

            if (mediaType == GraphFormatter.mediaTypes["n3"])
            {
                return new Notation3Writer();
            }

            if (mediaType == GraphFormatter.mediaTypes["ntriples"])
            {
                return new NTriplesWriter();
            }

            if (mediaType == GraphFormatter.mediaTypes["rdfxml"])
            {
                return new RdfXmlWriter() { UseDtd = false };
            }

            if (mediaType == GraphFormatter.mediaTypes["rdfjson"])
            {
                return new RdfJsonWriter();
            }

            if (mediaType == GraphFormatter.mediaTypes["tsv"])
            {
                return new TsvWriter();
            }

            if (mediaType == GraphFormatter.mediaTypes["turtle"])
            {
                return new CompressingTurtleWriter();
            }

            throw new NotSupportedException();
        }
    }
}
