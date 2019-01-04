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