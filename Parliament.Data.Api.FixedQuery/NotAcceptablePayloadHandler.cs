namespace Parliament.Data.Api.FixedQuery
{
    using Newtonsoft.Json.Linq;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using VDS.RDF;
    using VDS.RDF.Query;

    public class NotAcceptablePayloadHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Let the framework do its job.
            var response = await base.SendAsync(request, cancellationToken);

            // Did content negotiation fail (i.e. no adequate formatter was found)?
            if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                var graphFormatters = request
                    .GetConfiguration()
                    .Formatters
                    .OfType<GraphFormatter>();

                var extensionMappings = graphFormatters
                    .Where(formatter => formatter.MediaTypeMappings.Single() is UriPathExtensionMapping)
                    .Select(formatter => formatter.MediaTypeMappings.Single() as UriPathExtensionMapping)
                    .Select(formatter => new
                    {
                        extension = formatter.UriPathExtension,
                        mimeType = formatter.MediaType.MediaType
                    })
                    .Distinct();

                var headerMappings = graphFormatters
                    .Where(formatter => formatter.MediaTypeMappings.Single() is RequestHeaderMapping)
                    .Select(formatter => formatter.MediaTypeMappings.Single() as RequestHeaderMapping)
                    .Select(formatter => formatter.MediaType.MediaType)
                    .Distinct();


                var responseText = new JObject(new JProperty(
                    "content negotiation", new JObject(new JProperty(
                        "supported methods", new JObject(new JProperty(
                            "file extension", new JObject(new JProperty(
                                "extension mapping", new JObject(extensionMappings.Select(definition => new JProperty(
                                    definition.extension, definition.mimeType)))), new JProperty(
                                "query string", new JObject(new JProperty(
                                    "parameter name", "format"))), new JProperty(
                                "accept header", new JObject(new JProperty(
                                    "known content types", headerMappings))))))))));

                response.Content = new StringContent(responseText.ToString(), Encoding.UTF8, "text/json");
            }

            return response;
        }
    }
}