namespace NewQuery
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    internal class QueryService : IQueryService
    {
        private readonly IDictionary<string, string> endpoints;

        internal QueryService()
        {
            this.endpoints = new Dictionary<string, string> { { "a", "A" } };
        }

        public virtual IActionResult Execute(string name, IQueryCollection parameters)
        {
            if (!this.endpoints.ContainsKey(name))
            {
                return new BadRequestResult();
            }

            return new OkObjectResult(this.endpoints[name]);
        }
    }
}
