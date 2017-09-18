namespace Parliament.Data.Api.FixedQuery
{
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