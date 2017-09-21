namespace Parliament.Data.Api.FixedQuery
{
    using System.IO;
    using System.Threading.Tasks;

    internal abstract class Compressor
    {
        internal abstract string EncodingType { get; }

        internal abstract Stream CreateCompressionStream(Stream output);

        internal virtual Task Compress(Stream source, Stream destination)
        {
            var compressed = this.CreateCompressionStream(destination);

            return source.CopyToAsync(compressed).ContinueWith(task => compressed.Dispose());
        }
    }
}