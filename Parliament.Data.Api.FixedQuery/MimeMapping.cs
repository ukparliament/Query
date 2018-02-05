namespace Parliament.Data.Api.FixedQuery
{
    using System;
    using System.Collections.Generic;
    using VDS.RDF;


    public class MimeMapping
    {
        public IEnumerable<string> MimeTypes { get; set; }

        public IEnumerable<string> Extensions { get; set; }

        public Func<IRdfWriter> RdfWriter { get; set; }

        public Func<IStoreWriter> StoreWriter { get; set; }

        public Func<ISparqlResultsWriter> SparqlWriter { get; set; }

        public bool CanWriteRdf
        {
            get
            {
                return this.RdfWriter != null;
            }
        }

        public bool CanWriteStore
        {
            get
            {
                return this.StoreWriter != null;
            }
        }

        public bool CanWriteSparql
        {
            get
            {
                return this.SparqlWriter != null;
            }
        }
    }
}