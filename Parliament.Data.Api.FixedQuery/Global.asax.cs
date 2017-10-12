namespace Parliament.Data.Api.FixedQuery
{
    using Microsoft.ApplicationInsights.Extensibility;
    using System;
    using System.Configuration;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Dispatcher;
    using System.Web.Http.ExceptionHandling;

    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var config = GlobalConfiguration.Configuration;

            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];

            var pipeline = HttpClientFactory.CreatePipeline(
                new HttpControllerDispatcher(config),
                new DelegatingHandler[] {
                    new CompressionHandler(),
                    new NotAcceptablePayloadHandler(),
                    new HtmlResourceRedirectHandler()
            });
            var constraints = new
            {
                name = new EndpointConstraint()
            };

            config.Routes.MapHttpRoute("Index", string.Empty, new { controller = "Help" });
            config.Routes.MapHttpRoute("WithExtension", "{name}.{ext}", new { controller = "FixedQuery" }, constraints, pipeline);
            config.Routes.MapHttpRoute("WithoutExtension", "{name}", new { controller = "FixedQuery", ext = string.Empty }, constraints, pipeline);
            config.Routes.MapHttpRoute("BadRequest", "{*any}", new { controller = "BadRequest" });

            config.Services.Add(typeof(IExceptionLogger), new AIExceptionLogger());

            config.Formatters.Clear();
            config.Formatters.Add(new HttpErrorJsonFormatter());
            config.Formatters.Add(new HttpErrorXmlFormatter());
        }
    }
}