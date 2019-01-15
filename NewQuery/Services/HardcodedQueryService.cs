namespace NewQuery
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    public class HardcodedQueryService : QueryService
    {
        public override IActionResult Execute(string name, IDictionary<string, string> parameters)
        {
            return base.Execute(name, parameters);
        }
    }
}
