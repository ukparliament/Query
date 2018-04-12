using System.Web.Http;
using WebActivatorEx;
using Parliament.Data.Api.FixedQuery;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Parliament.Data.Api.FixedQuery
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                 {
                     c.SingleApiVersion("v1", "Fixed Query API");
                     c.DocumentFilter<FixedQueryDocumentFilter>();
                 })
                .EnableSwaggerUi();
        }
    }
}
