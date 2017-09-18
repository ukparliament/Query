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

            var chosenCompressor = request.Headers.AcceptEncoding
                .Join(
                    compressors,
                    acceptHeader => acceptHeader.Value,
                    compressor => compressor.EncodingType,
                    (acceptHeader, compressor) => compressor)
                .FirstOrDefault();

            if (chosenCompressor != null)
            {
                response.Content = new CompressedContent(response.Content, chosenCompressor);
            }

            return response;
        }
    }
}