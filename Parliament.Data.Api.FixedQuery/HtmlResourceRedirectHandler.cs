namespace Parliament.Data.Api.FixedQuery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    internal class HtmlResourceRedirectHandler : DelegatingHandler
    {
        private const string resourceEndpointTemplate = "https://beta.parliament.uk/resource/{0}";
        private const string namespaceBase = "https://id.parliament.uk/";
        private const string resourceEndpointName = "resource";
        private const string endpointNameRouteParameter = "name";

        private HttpRequestMessage request;
        private HttpResponseMessage response;

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.request = request;
            this.response = await base.SendAsync(this.request, cancellationToken);

            if (this.IsResourceLookupRequest && !this.Stay && this.IsHtmlRequest)
            {
                return this.Redirect();
            }

            return this.response;
        }

        private bool IsResourceLookupRequest
        {
            get
            {
                var routeData = this.request.GetRouteData();
                var endpointName = routeData.Values[HtmlResourceRedirectHandler.endpointNameRouteParameter] as string;

                return endpointName == HtmlResourceRedirectHandler.resourceEndpointName;
            }
        }

        private bool IsHtmlRequest
        {
            get
            {
                var config = this.request.GetConfiguration();
                var negotiator = config.Services.GetContentNegotiator();
                var result = negotiator.Negotiate((this.response.Content as ObjectContent).ObjectType, this.request, config.Formatters);

                var bestMatchFormatter = result.Formatter;
                var mediaType = result.MediaType.MediaType;

                return mediaType == "text/html";
            }
        }

        private HttpResponseMessage Redirect()
        {
            var response = this.request.CreateResponse(HttpStatusCode.Found);
            var redirectUri = string.Format(HtmlResourceRedirectHandler.resourceEndpointTemplate, this.ResourceId);
            response.Headers.Location = new Uri(redirectUri);

            return response;
        }

        private Uri ResourceId
        {
            get
            {
                var resourceUri = new Uri(this.Parameters["uri"]);

                var namespaceUri = new Uri(HtmlResourceRedirectHandler.namespaceBase);
                var resourceId = namespaceUri.MakeRelativeUri(resourceUri);

                return resourceId;
            }
        }

        private Dictionary<string, string> Parameters => this.request.GetQueryNameValuePairs().ToDictionary(pair => pair.Key, pair => pair.Value);

        private bool Stay => this.Parameters.ContainsKey("stay");
    }
}