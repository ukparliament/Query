namespace Parliament.Data.Api.FixedQuery
{
    using System;
    using VDS.RDF.Query;

    [Serializable]
    public class SparqlInvalidException : RdfQueryException
    {
        public SparqlInvalidException(string message) : base(message) { }
    }
}