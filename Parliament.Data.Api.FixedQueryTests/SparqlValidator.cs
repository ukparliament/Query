namespace Parliament.Data.Api.FixedQueryTests
{
    using Parliament.Data.Api.FixedQuery;
    using System;

    public class SparqlValidator
    {
        protected void ValidateSparql(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e) when (!(e is SparqlInvalidException)) { }
        }
    }
}
