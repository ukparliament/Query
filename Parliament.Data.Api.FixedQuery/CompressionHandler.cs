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