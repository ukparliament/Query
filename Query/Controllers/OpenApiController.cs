namespace Query
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;

    public class OpenApiController : Controller
    {
        [HttpGet("openapi.json")]
        public JObject Get()
        {
            return Resources.OpenApiDocument;
        }
    }
}
