namespace Parliament.Data.Api.FixedQuery
{
    using System.IO;
    using System.IO.Compression;
    internal class GzipCompressor : Compressor
    {
        internal override string EncodingType => "gzip";

        internal override Stream CreateCompressionStream(Stream output)
        {
            return new GZipStream(output, CompressionMode.Compress);
        }
    }
}