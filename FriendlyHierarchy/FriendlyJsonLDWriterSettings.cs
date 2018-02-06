namespace FriendlyHierarchy
{
    public class FriendlyJsonLDWriterSettings : SpanningAlgorithmSettings
    {
        // TODO: Implement?
        public readonly bool compactBase = false;
        public readonly bool compactVocab = false;

        public FriendlyJsonLDWriterSettings() : base()
        {
            this.AlwaysUseArrays = false;
            this.AlwaysUseStructuredLiterals = false;
            this.CompactUris = true;
        }

        public bool AlwaysUseArrays { get; set; }

        public bool AlwaysUseStructuredLiterals { get; set; }

        public bool CompactUris { get; set; }
    }
}