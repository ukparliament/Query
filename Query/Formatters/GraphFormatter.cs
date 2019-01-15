namespace Query
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using VDS.RDF;
    using VDS.RDF.Query;
    using VDS.RDF.Writing;

    public class GraphFormatter : TextOutputFormatter
    {
        private readonly (string[] MediaTypes, string[] Extensions, Func<IRdfWriter> rdfWriter, Func<ISparqlResultsWriter> sparqlWriter, Func<IStoreWriter> storeWriter) mapping;

        public GraphFormatter((string[] MediaTypes, string[] Extensions, Func<IRdfWriter> rdfWriter, Func<ISparqlResultsWriter> sparqlWriter, Func<IStoreWriter> storeWriter) mapping)
        {
            this.mapping = mapping;

            foreach (var item in mapping.MediaTypes)
            {
                this.SupportedMediaTypes.Add(item);
            }

            this.SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type type)
        {
            return !(this.mapping.rdfWriter is null && this.mapping.storeWriter is null) && typeof(IGraph).IsAssignableFrom(type) ||
                !(this.mapping.sparqlWriter is null) && typeof(SparqlResultSet).IsAssignableFrom(type);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            return new TaskFactory().StartNew(() =>
            {
                using (var writer = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
                {
                    if (context.Object is IGraph graph)
                    {
                        if (!(this.mapping.storeWriter is null))
                        {
                            var store = new TripleStore();
                            store.Add(graph);

                            this.mapping.storeWriter().Save(store, writer);
                        }
                        else
                        {
                            var rdfWriter = this.mapping.rdfWriter();

                            if (rdfWriter is HtmlWriter htmlWriter)
                            {
                                //htmlWriter.UriPrefix = "resource?stay&uri=";
                            }

                            rdfWriter.Save(graph, writer);
                        }
                    }
                    else if (!(this.mapping.sparqlWriter is null))
                    {
                        var sparqlWriter = this.mapping.sparqlWriter();

                        if (sparqlWriter is SparqlHtmlWriter htmlWriter)
                        {
                            htmlWriter.UriPrefix = "resource?uri=";
                        }

                        sparqlWriter.Save(context.Object as SparqlResultSet, writer);
                    }
                }
            });
        }
    }
}
