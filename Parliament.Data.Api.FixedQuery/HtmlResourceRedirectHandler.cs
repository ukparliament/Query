// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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

                var resourceId = Global.InstanceUri.MakeRelativeUri(resourceUri);

                return resourceId;
            }
        }

        private Dictionary<string, string> Parameters => this.request.GetQueryNameValuePairs().ToDictionary(pair => pair.Key, pair => pair.Value);

        private bool Stay => this.Parameters.ContainsKey("stay");
    }
}