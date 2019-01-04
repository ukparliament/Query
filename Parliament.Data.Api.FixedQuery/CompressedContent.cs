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
    using Microsoft.ApplicationInsights;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal class CompressedContent : HttpContent
    {
        private readonly HttpContent content;

        private readonly Compressor compressor;

        internal CompressedContent(HttpContent content, Compressor compressor)
        {
            this.content = content;
            this.compressor = compressor;

            this.AddHeaders();
        }

        protected async override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            using (this.content)
            {
                var contentStream = await this.content.ReadAsStreamAsync();

                await this.compressor.Compress(contentStream, stream);

                new TelemetryClient().TrackEvent(
                    "Compressed",
                    new Dictionary<string, string> {{
                        "encodingType",
                        this.compressor.EncodingType } });
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;

            return false;
        }

        private void AddHeaders()
        {
            foreach (var header in this.content.Headers)
            {
                this.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            this.Headers.ContentEncoding.Add(this.compressor.EncodingType);
        }
    }
}