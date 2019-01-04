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
    using FriendlyHierarchy;
    using Microsoft.ApplicationInsights.Extensibility;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System.Web.Http.Dispatcher;
    using VDS.RDF;
    using VDS.RDF.Parsing;
    using VDS.RDF.Writing;

    public class Global : HttpApplication
    {
        public static readonly Uri InstanceUri = new Uri("https://id.parliament.uk/");
        public static readonly Uri SchemaUri = new Uri(InstanceUri, "schema/");

        internal static IEnumerable<MimeMapping> MimeTypeDefinitions
        {
            get
            {
                return new[] {
                    new MimeMapping {
                        MimeTypes = new[] { "application/n-triples" },
                        Extensions = new[] { "nt" },
                        RdfWriter = () => new NTriplesWriter()
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "text/turtle" },
                        Extensions = new[] { "ttl" },
                        RdfWriter = () => new CompressingTurtleWriter()
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "application/sparql-results+xml" },
                        Extensions = new[] { "srx" },
                        SparqlWriter = () => new SparqlXmlWriter()
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "application/sparql-results+json" },
                        Extensions = new[] { "srj" },
                        SparqlWriter = () => new SparqlJsonWriter()
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "text/csv" },
                        Extensions = new[] { "csv" },
                        RdfWriter = () => new CsvWriter(),
                        SparqlWriter = () => new SparqlCsvWriter(),
                        StoreWriter = () => new CsvStoreWriter()
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "text/tab-separated-values" },
                        Extensions = new[] { "tsv" },
                        RdfWriter = () => new TsvWriter(),
                        SparqlWriter = () => new SparqlTsvWriter(),
                        StoreWriter = () => new TsvStoreWriter()
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "text/html", "application/xhtml+xml" },
                        Extensions = new[] { "html", "htm", "xhtml" },
                        RdfWriter = () => new HtmlWriter(),
                        SparqlWriter = () => new SparqlHtmlWriter()
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "text/vnd.graphviz" },
                        Extensions = new[] { "gv", "dot" },
                        RdfWriter = () => new GraphVizWriter() { CollapseLiterals = false }
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "application/rdf+xml", "application/xml", "text/xml" },
                        Extensions = new[] { "xml", "rdf", "rdfxml" },
                        RdfWriter = () => new FriendlyRdfXmlWriter(new FriendlyRdfXmlWriterSettings{ BaseUri = Global.InstanceUri, VocabularyUri = Global.SchemaUri }),
                        SparqlWriter = () => new SparqlXmlWriter()
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "application/ld+json", "application/json" },
                        Extensions = new[] { "json", "jsonld" },
                        RdfWriter = () => new FriendlyJsonLdWriter(new FriendlyJsonLDWriterSettings{ BaseUri = Global.InstanceUri, VocabularyUri = Global.SchemaUri }),
                        SparqlWriter = () => new SparqlJsonWriter()
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "application/rdf+json" },
                        Extensions = new[] { "rj" },
                        RdfWriter = () => new RdfJsonWriter()
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "application/graphml+xml" },
                        Extensions = new[] { "graphml" },
                        StoreWriter = () => new GraphMLWriter() { CollapseLiterals = false }
                    }
                };
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            var config = GlobalConfiguration.Configuration;

            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];

            Configure(config);
        }

        public static void Configure(HttpConfiguration config)
        {
            var pipeline = HttpClientFactory.CreatePipeline(
                new HttpControllerDispatcher(config),
                new DelegatingHandler[] {
                    new CompressionHandler(),
                    new NotAcceptablePayloadHandler(),
                    //new HtmlResourceRedirectHandler()
            });
            var constraints = new
            {
                name = new EndpointConstraint()
            };

            config.Routes.MapHttpRoute("Index", string.Empty, new { controller = "Help" });
            config.Routes.MapHttpRoute("OpenApiDefinition", "openapi.json", new { controller = "OpenApiDefinition" });
            config.Routes.MapHttpRoute("WithExtension", "{name}.{ext}", new { controller = "FixedQuery" }, constraints, pipeline);
            config.Routes.MapHttpRoute("WithoutExtension", "{name}", new { controller = "FixedQuery", ext = string.Empty }, constraints, pipeline);
            config.Routes.MapHttpRoute("BadRequest", "{*any}", new { controller = "BadRequest" });

            config.Formatters.Clear();
            config.Formatters.Add(new HttpErrorJsonFormatter());
            config.Formatters.Add(new HttpErrorXmlFormatter());
        }
    }
}