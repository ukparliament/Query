// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Parliament.Data.Api.FixedQuery
{
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Text;
    using VDS.RDF;
    using VDS.RDF.Query;
    using VDS.RDF.Writing;

    public class GraphFormatter : BufferedMediaTypeFormatter
    {
        private MimeMapping definition;

        private MimeMapping Definition
        {
            get
            {
                var mapping = this.MediaTypeMappings.Single();
                if (mapping is UriPathExtensionMapping extensionMapping)
                {
                    this.definition = Global.MimeTypeDefinitions.Single(x => x.Extensions.Contains(extensionMapping.UriPathExtension));
                }
                else
                {
                    this.definition = Global.MimeTypeDefinitions.Single(x => x.MimeTypes.Contains(mapping.MediaType.MediaType));
                }

                return this.definition;
            }
        }

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
            return (this.Definition.CanWriteRdf || this.Definition.CanWriteStore) && typeof(IGraph).IsAssignableFrom(type)
                || this.Definition.CanWriteSparql && typeof(SparqlResultSet).IsAssignableFrom(type);
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            var streamWriter = new StreamWriter(writeStream, new UTF8Encoding());

            if (value is IGraph graph)
            {
                if (this.Definition.CanWriteStore)
                {
                    var store = new TripleStore();
                    store.Add(graph);

                    this.Definition.StoreWriter().Save(store, streamWriter);
                }
                else
                {
                    var writer = this.Definition.RdfWriter();

                    if (writer is HtmlWriter htmlWriter)
                    {
                        //htmlWriter.UriPrefix = "resource?stay&uri=";
                    }

                    writer.Save(graph, streamWriter);
                }
            }
            else if (this.Definition.CanWriteSparql)
            {
                var writer = this.Definition.SparqlWriter();
                if (writer is SparqlHtmlWriter htmlWriter)
                {
                    htmlWriter.UriPrefix = "resource?uri=";
                }

                writer.Save(value as SparqlResultSet, streamWriter);
            }

            streamWriter.Close();

            GraphFormatter.TrackWriteEvent(this.SupportedMediaTypes.Single().MediaType);
        }

        private static void TrackWriteEvent(string format)
        {
            var telemetry = new EventTelemetry("Write");
            telemetry.Properties.Add("format", format);

            // This makes the event part of the overall request context.
            new OperationCorrelationTelemetryInitializer().Initialize(telemetry);

            new TelemetryClient().TrackEvent(telemetry);
        }
    }
}
