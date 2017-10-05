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
    using VDS.RDF;
    using VDS.RDF.Query;
    using VDS.RDF.Writing;

    public class GraphFormatter : BufferedMediaTypeFormatter
    {
        private MimeTypeDefinition definition;

        private MimeTypeDefinition Definition
        {
            get
            {
                if (definition == null)
                {
                    var mapping = this.MediaTypeMappings.Single();

                    var extensionMapping = mapping as UriPathExtensionMapping;
                    if (extensionMapping != null)
                    {
                        this.definition = MimeTypesHelper.GetDefinitionsByFileExtension(extensionMapping.UriPathExtension).First();
                    }
                    else
                    {
                        this.definition = MimeTypesHelper.GetDefinitions(mapping.MediaType.MediaType).First();
                    }
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
            return (this.Definition.CanWriteRdf || this.Definition.CanWriteRdfDatasets) && typeof(IGraph).IsAssignableFrom(type)
                || this.Definition.CanWriteSparqlResults && typeof(SparqlResultSet).IsAssignableFrom(type);
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            var streamWriter = new StreamWriter(writeStream);

            if (this.Definition.CanWriteRdf)
            {
                var writer = this.Definition.GetRdfWriter();

                if (writer is HtmlWriter)
                {
                    (writer as HtmlWriter).UriPrefix = "resource_by_id?resource_id=";
                }

                writer.Save(value as IGraph, streamWriter);
            }
            else if (this.Definition.CanWriteSparqlResults)
            {
                this.Definition.GetSparqlResultsWriter().Save(value as SparqlResultSet, streamWriter);
            }
            else if (this.Definition.CanWriteRdfDatasets)
            {
                var store = new TripleStore();
                store.Add(value as IGraph);

                this.Definition.GetRdfDatasetWriter().Save(store, streamWriter);
            }

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
