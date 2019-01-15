namespace NewQuery
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;

    public class QueryController : ControllerBase
    {
        private readonly IQueryService queryService;

        public QueryController(IQueryService queryService)
        {
            this.queryService = queryService;
        }

        [HttpGet("{name}")]
        [HttpGet("{name}.{format:query}")]
        [FormatFilter]
        public IActionResult Get(string name)
        {
            return this.queryService.Execute(name, this.Request.Query.ToDictionary(kv => kv.Key, kv => kv.Value.ToString()));
        }
    }
}
