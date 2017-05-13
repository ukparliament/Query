namespace Parliament.Data.Api.FixedQuery
{
    using System;

    [Serializable]
    public class SparqlInvalidException : Exception
    {
        public SparqlInvalidException(string message) : base(message) { }
    }
}