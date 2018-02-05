namespace FriendlyHierarchy
{
    using System;

    public class SpanningAlgorithmSettings
    {
        public SpanningAlgorithmSettings()
        {
            this.LiftMultiParents = true;
            this.Flat = false;
            this.UseLists = true;
            this.LiftTypes = true;
        }

        public Uri BaseUri { get; set; }

        public Uri VocabularyUri { get; set; }

        public bool Flat { get; set; }

        public bool LiftMultiParents { get; set; }

        public bool UseLists { get; set; }

        public bool LiftTypes { get; set; }
    }
}