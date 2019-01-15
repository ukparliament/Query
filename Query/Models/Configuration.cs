namespace Query
{
    using System;
    using FriendlyHierarchy;
    using Microsoft.OpenApi.Writers;
    using VDS.RDF;
    using VDS.RDF.Writing;

    public class Configuration
    {
        public static readonly Uri InstanceUri = new Uri("https://id.parliament.uk/");
        public static readonly Uri SchemaUri = new Uri(InstanceUri, "schema/");

        public static readonly (string[] MediaTypes, string[] Extensions, Func<IRdfWriter> rdfWriter, Func<ISparqlResultsWriter> sparqlWriter, Func<IStoreWriter> storeWriter)[] QueryMappings = new (string[] MediaTypes, string[] Extensions, Func<IRdfWriter> rdfWriter, Func<ISparqlResultsWriter> sparqlWriter, Func<IStoreWriter> storeWriter)[] {
            (new[] { "application/n-triples" }, new[] { "nt" }, () => new NTriplesWriter(), null, null),
            (new[] { "text/turtle" }, new[] { "ttl" }, () => new CompressingTurtleWriter(), null, null),
            (new[] { "application/sparql-results+xml" }, new[] { "srx" }, null, () => new SparqlXmlWriter(), null),
            (new[] { "application/sparql-results+json" }, new[] { "srj" }, null, () => new SparqlJsonWriter(), null),
            (new[] { "text/csv" }, new[] { "csv" }, () => new CsvWriter(), () => new SparqlCsvWriter(), () => new CsvStoreWriter()),
            (new[] { "text/tab-separated-values" }, new[] { "tsv" }, () => new TsvWriter(), () => new SparqlTsvWriter(), () => new TsvStoreWriter()),
            (new[] { "text/html", "application/xhtml+xml" }, new[] { "html", "htm", "xhtml" }, () => new HtmlWriter(), () => new SparqlHtmlWriter(), null),
            (new[] { "text/vnd.graphviz" }, new[] { "gv", "dot" }, () => new GraphVizWriter() { CollapseLiterals = false }, null, null),
            (new[] { "application/rdf+xml", "application/xml", "text/xml" }, new[] { "xml", "rdf", "rdfxml" }, () => new FriendlyRdfXmlWriter(new FriendlyRdfXmlWriterSettings{ BaseUri = Configuration.InstanceUri, VocabularyUri = Configuration.SchemaUri }), () => new SparqlXmlWriter(), null),
            (new[] { "application/ld+json", "application/json" }, new[] { "json", "jsonld" }, () => new FriendlyJsonLdWriter(new FriendlyJsonLDWriterSettings{ BaseUri = Configuration.InstanceUri, VocabularyUri = Configuration.SchemaUri }), () => new SparqlJsonWriter(), null),
            (new[] { "application/rdf+json" }, new[] { "rj" }, () => new RdfJsonWriter(), null, null),
            (new[] { "application/graphml+xml" }, new[] { "graphml" }, () => null, null, () => new GraphMLWriter() { CollapseLiterals = false })
        };

        internal static readonly (string MediaType, string Extension, Type WriterType)[] OpenApiMappings = new[] {
            ("application/json", "json", typeof(OpenApiJsonWriter)),
            ("text/vnd.yaml", "yaml", typeof(OpenApiYamlWriter))
        };
    }
}
