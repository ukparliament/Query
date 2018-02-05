namespace FriendlyHierarchy
{
    using VDS.RDF;

    internal class JsonLd
    {
        public const string Context = "@context";
        public const string Base = "@base";
        public const string Vocab = "@vocab";
        public const string Graph = "@graph";
        public const string Type = "@type";
        public const string Id = "@id";
        public const string List = "@list";
        public const string Value = "@value";
        public const string Language = "@language";
        public const string Container = "@container";
    }

    internal class RdfXml
    {
        public const string Namespace = NamespaceMapper.RDF;
        public const string Prefix = "rdf";
        public const string RDF = "RDF";
        public const string Description = "Description";
        public const string Resource = "resource";
        public const string ParseType = "parseType";
        public const string NodeId = "nodeID";
        public const string Datatype = "datatype";
        public const string About = "about";
        public const string Collection = "Collection";
    }

    internal class XmlNs
    {
        public const string Prefix = "xmlns";
    }

    internal class Xml
    {
        public const string Prefix = "xml";
        public const string Lang = "lang";
        public const string Base = "base";
    }
}
