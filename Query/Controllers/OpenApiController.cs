namespace Query
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.OpenApi.Models;

    public class OpenApiController : Controller
    {
        [HttpGet("openapi")]
        [HttpGet("openapi.{format:openapi}")]
        [FormatFilter]
        public OpenApiDocument Get()
        {
            return Resources.OpenApiDocument;
        }
    }
}
