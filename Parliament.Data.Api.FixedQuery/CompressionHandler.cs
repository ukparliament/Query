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
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    internal class CompressionHandler : DelegatingHandler
    {
        private readonly IEnumerable<Compressor> compressors = new List<Compressor> {
            new GzipCompressor(),
            new DeflateCompressor()
        };

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.Content != null)
            {
                var chosenCompressor = request.Headers.AcceptEncoding
                    .OrderByDescending(acceptHeader => acceptHeader.Quality)
                    .Join(
                        this.compressors,
                        acceptHeader => acceptHeader.Value,
                        compressor => compressor.EncodingType,
                        (acceptHeader, compressor) => compressor,
                        new CompressionHandler.Comparer())
                    .FirstOrDefault();

                if (chosenCompressor != null)
                {
                    response.Content = new CompressedContent(response.Content, chosenCompressor);
                }
            }

            return response;
        }

        private class Comparer : IEqualityComparer<string>
        {
            private const string all = "*";

            public bool Equals(string x, string y)
            {
                return EqualityComparer<string>.Default.Equals(x, y) || x == Comparer.all || y == Comparer.all;
            }

            public int GetHashCode(string obj)
            {
                return Comparer.all.GetHashCode();
            }
        }
    }
}