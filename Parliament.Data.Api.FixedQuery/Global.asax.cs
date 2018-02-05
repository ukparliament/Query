namespace Parliament.Data.Api.FixedQuery
{
    using Microsoft.ApplicationInsights.Extensibility;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Dispatcher;
    using System.Web.Http.ExceptionHandling;
    using VDS.RDF;
    using VDS.RDF.Writing;

    public class Global : HttpApplication
    {
        internal static IEnumerable<MimeMapping> MimeTypeDefinitions
        {
            get
            {
                return new[] {
                    new MimeMapping {
                        MimeTypes = new[] { "application/n-triples" },
                        Extensions = new[] { "nt" },
                        RdfWriter = new Func<IRdfWriter>(() => new NTriplesWriter())
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "text/turtle" },
                        Extensions = new[] { "ttl" },
                        RdfWriter = new Func<IRdfWriter>(() => new CompressingTurtleWriter())
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "application/sparql-results+xml" },
                        Extensions = new[] { "srx" },
                        SparqlWriter = new Func<ISparqlResultsWriter>(() => new SparqlXmlWriter())
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "application/sparql-results+json" },
                        Extensions = new[] { "srj" },
                        SparqlWriter = new Func<ISparqlResultsWriter>(() => new SparqlJsonWriter())
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "text/csv" },
                        Extensions = new[] { "csv" },
                        RdfWriter = new Func<IRdfWriter>(() => new CsvWriter()),
                        SparqlWriter = new Func<ISparqlResultsWriter>(() => new SparqlCsvWriter()),
                        StoreWriter = new Func<IStoreWriter>(() => new CsvStoreWriter())
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "text/tab-separated-values" },
                        Extensions = new[] { "tsv" },
                        RdfWriter = new Func<IRdfWriter>(() => new TsvWriter()),
                        SparqlWriter = new Func<ISparqlResultsWriter>(() => new SparqlTsvWriter()),
                        StoreWriter = new Func<IStoreWriter>(() => new TsvStoreWriter())
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "text/html", "application/xhtml+xml" },
                        Extensions = new[] { "html", "htm", "xhtml" },
                        RdfWriter = new Func<IRdfWriter>(() => new HtmlWriter()),
                        SparqlWriter = new Func<ISparqlResultsWriter>(() => new SparqlHtmlWriter())
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "text/vnd.graphviz" },
                        Extensions = new[] { "gv", "dot" },
                        RdfWriter = new Func<IRdfWriter>(() => new GraphVizWriter())
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "application/rdf+xml", "application/xml", "text/xml" },
                        Extensions = new[] { "xml", "rdf", "rdfxml" },
                        RdfWriter = new Func<IRdfWriter>(() => new FriendlyHierarchy.FriendlyRdfXmlWriter(new FriendlyHierarchy.FriendlyRdfXmlWriterSettings{ BaseUri = new Uri("https://id.parliament.uk/"), VocabularyUri = new Uri("https://id.parliament.uk/schema/") })),
                        SparqlWriter = new Func<ISparqlResultsWriter>(() => new SparqlXmlWriter())
                    },
                    new MimeMapping {
                        MimeTypes = new[] { "application/json+ld", "application/json" },
                        Extensions = new[] { "json", "jsonld" },
                        RdfWriter = new Func<IRdfWriter>(() => new FriendlyHierarchy.FriendlyJsonLdWriter(new FriendlyHierarchy.FriendlyJsonLDWriterSettings{ BaseUri = new Uri("https://id.parliament.uk/"), VocabularyUri = new Uri("https://id.parliament.uk/schema/") })),
                        SparqlWriter = new Func<ISparqlResultsWriter>(() => new SparqlJsonWriter())
                    }
                };
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            var config = GlobalConfiguration.Configuration;

            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];

            var pipeline = HttpClientFactory.CreatePipeline(
                new HttpControllerDispatcher(config),
                new DelegatingHandler[] {
                    new CompressionHandler(),
                    new NotAcceptablePayloadHandler(),
                    new HtmlResourceRedirectHandler()
            });
            var constraints = new
            {
                name = new EndpointConstraint()
            };

            config.Routes.MapHttpRoute("Index", string.Empty, new { controller = "Help" });
            config.Routes.MapHttpRoute("WithExtension", "{name}.{ext}", new { controller = "FixedQuery" }, constraints, pipeline);
            config.Routes.MapHttpRoute("WithoutExtension", "{name}", new { controller = "FixedQuery", ext = string.Empty }, constraints, pipeline);
            config.Routes.MapHttpRoute("BadRequest", "{*any}", new { controller = "BadRequest" });

            config.Services.Add(typeof(IExceptionLogger), new AIExceptionLogger());

            config.Formatters.Clear();
            config.Formatters.Add(new HttpErrorJsonFormatter());
            config.Formatters.Add(new HttpErrorXmlFormatter());
        }
    }
}