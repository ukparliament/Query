namespace Parliament.Data.Api.FixedQuery
{
    using Microsoft.ApplicationInsights.Extensibility;
    using System;
    using System.Configuration;
    using System.Net.Http.Formatting;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;

    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];

            var config = GlobalConfiguration.Configuration;

            config.Services.Replace(typeof(IContentNegotiator), new DefaultContentNegotiator(true));

            config.Formatters.Clear();

            config.Formatters.Add(new GraphFormatter(new UriPathExtensionMapping("ttl", "text/turtle")));
            config.Formatters.Add(new GraphFormatter(new UriPathExtensionMapping("json", "application/rdf+json")));
            config.Formatters.Add(new GraphFormatter(new UriPathExtensionMapping("xml", "application/rdf+xml")));

            config.Formatters.Add(new GraphFormatter(new QueryStringMapping("format", "application/rdf+json", "application/rdf+json")));
            config.Formatters.Add(new GraphFormatter(new QueryStringMapping("format", "application/json", "application/rdf+json")));
            config.Formatters.Add(new GraphFormatter(new QueryStringMapping("format", "text/json", "application/rdf+json")));
            config.Formatters.Add(new GraphFormatter(new QueryStringMapping("format", "application/xml+rdf", "application/rdf+xml")));
            config.Formatters.Add(new GraphFormatter(new QueryStringMapping("format", "application/xml", "application/rdf+xml")));
            config.Formatters.Add(new GraphFormatter(new QueryStringMapping("format", "text/xml", "application/rdf+xml")));

            config.Formatters.Add(new GraphFormatter(new RequestHeaderMapping("Accept", "text/turtle", StringComparison.Ordinal, false, "text/turtle")));
            config.Formatters.Add(new GraphFormatter(new RequestHeaderMapping("Accept", "application/rdf+json", StringComparison.Ordinal, false, "application/rdf+json")));
            config.Formatters.Add(new GraphFormatter(new RequestHeaderMapping("Accept", "application/json", StringComparison.Ordinal, false, "application/rdf+json")));

            config.Formatters.Add(new GraphFormatter(new RequestHeaderMapping("Accept", "text/html", StringComparison.Ordinal, false, "text/html")));


            config.Formatters.Add(new HttpErrorJsonFormatter());
            config.Formatters.Add(new HttpErrorXmlFormatter());

            //config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("WithExtension", "{action}.{ext}", new { controller = "x" });
            config.Routes.MapHttpRoute("WithoutExtension", "{action}", new { controller = "x" });

            config.Routes.MapHttpRoute("BadRequest", "{*any}", new { controller = "BadRequest", action = "Get" });

            config.Services.Add(typeof(IExceptionLogger), new AIExceptionLogger());

            config.EnsureInitialized();
        }
    }
}