namespace Parliament.Data.Api.FixedQuery
{
    using System.IO;
    using System.IO.Compression;

    internal class DeflateCompressor : Compressor
    {
        internal override string EncodingType => "deflate";

        internal override Stream CreateCompressionStream(Stream output)
        {
            return new DeflateStream(output, CompressionMode.Compress);
        }
    }
}