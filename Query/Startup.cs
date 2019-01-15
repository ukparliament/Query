namespace Query
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Swashbuckle.AspNetCore.SwaggerUI;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(Startup.SetupMvc);
            services.Configure<RouteOptions>(Startup.ConfigureRouteOptions);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRewriter(new RewriteOptions().AddRewrite("^$", "swagger/index.html", false).AddRewrite("^(swagger|favicon)(.+)$", "swagger/$1$2", true));
            app.UseMvc();
            app.UseSwaggerUI(Startup.ConfigureSwaggerUI);
        }

        private static void SetupMvc(MvcOptions mvc)
        {
            mvc.RespectBrowserAcceptHeader = true;

            foreach (var mapping in Configuration.QueryMappings)
            {
                mvc.OutputFormatters.Insert(0, new GraphFormatter(mapping));

                foreach (var extension in mapping.Extensions)
                {
                    mvc.FormatterMappings.SetMediaTypeMappingForFormat(extension, mapping.MediaTypes[0]);
                }

                foreach (var mediaType in mapping.MediaTypes)
                {
                    mvc.FormatterMappings.SetMediaTypeMappingForFormat(mediaType, mapping.MediaTypes[0]);
                }
            }

            foreach (var mapping in Configuration.OpenApiMappings)
            {
                mvc.OutputFormatters.Insert(0, new OpenApiFormatter(mapping.MediaType, mapping.WriterType));
                mvc.FormatterMappings.SetMediaTypeMappingForFormat(mapping.Extension, mapping.MediaType);
                mvc.FormatterMappings.SetMediaTypeMappingForFormat(mapping.MediaType, mapping.MediaType);
            }
        }

        private static void ConfigureRouteOptions(RouteOptions routes)
        {
            routes.ConstraintMap.Add("query", typeof(QueryExtensionConstraint));
            routes.ConstraintMap.Add("openapi", typeof(OpenApiExtensionConstraint));
        }

        private static void ConfigureSwaggerUI(SwaggerUIOptions swaggerUI)
        {
            swaggerUI.DocumentTitle = "UK Parliament Query Service";
            swaggerUI.SwaggerEndpoint("./openapi", "live");
        }
    }
}
