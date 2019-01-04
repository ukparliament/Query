// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
