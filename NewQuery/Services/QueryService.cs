namespace NewQuery
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    public class QueryService : IQueryService
    {
        public virtual IActionResult Execute(string name, IDictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }
    }
}
