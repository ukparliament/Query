namespace NewQuery
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    public interface IQueryService
    {
        IActionResult Execute(string name, IDictionary<string, string> parameters);
    }
}
