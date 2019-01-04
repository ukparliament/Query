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
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using VDS.RDF;
    using VDS.RDF.Parsing;
    using VDS.RDF.Writing;

    public class FriendlyRdfXmlWriter : SpanningAlgorithm, IRdfWriter
    {
        private XmlWriter writer;
        private FriendlyRdfXmlWriterSettings Settings
        {
            get
            {
                return base.settings as FriendlyRdfXmlWriterSettings;
            }
        }

        public FriendlyRdfXmlWriter() : this(new FriendlyRdfXmlWriterSettings())
        {
        }

        public FriendlyRdfXmlWriter(FriendlyRdfXmlWriterSettings settings) : base(settings)
        {
        }

        #region IRdfWriter
        public event RdfWriterWarning Warning;

        public void Save(IGraph g, string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                this.Save(g, writer);
            }
        }

        public void Save(IGraph g, TextWriter output)
        {
            this.Save(g, output, true);
        }

        public void Save(IGraph g, TextWriter output, bool leaveOpen)
        {
            this.writer = new XmlTextWriter(output) { Formatting = Formatting.Indented };
            this.Initialize(g);
            this.WriteRdf();
        }
        #endregion

        protected override void WriteRdf()
        {
            this.writer.WriteStartElement(RdfXml.Prefix, RdfXml.RDF, RdfXml.Namespace);

            if (this.Settings.VocabularyUri != null)
            {
                this.writer.WriteAttributeString(XmlNs.Prefix, XmlSpecsHelper.NamespaceXmlNamespaces, this.Settings.VocabularyUri.AbsoluteUri);
            }

            if (this.Settings.BaseUri != null)
            {
                this.writer.WriteAttributeString(Xml.Prefix, Xml.Base, XmlSpecsHelper.NamespaceXml, this.Settings.BaseUri.AbsoluteUri);
            }

            base.WriteRdf();

            this.writer.WriteEndElement();
        }

        protected override void WriteSubject(INode node)
        {
            if (!this.Settings.LiftTypes || !this.TryWritingTypedElement(node))
            {
                this.writer.WriteStartElement(RdfXml.Prefix, RdfXml.Description, RdfXml.Namespace);
            }

            base.WriteSubject(node);

            this.writer.WriteEndElement();
        }

        protected override void WriteSubjectId(INode node)
        {
            this.WriteId(node, RdfXml.About);
        }

        protected override void WritePredicate(Triple statement)
        {
            if (!this.TryWritingStartElement(statement.Predicate as IUriNode))
            {
                throw new RdfOutputException(WriterErrorMessages.UnreducablePropertyURIUnserializable + " - '" + statement.Predicate.ToString() + "'");
            }

            base.WritePredicate(statement);

            this.writer.WriteEndElement();
        }

        protected override void WriteList(INode node)
        {
            this.writer.WriteAttributeString(RdfXml.Prefix, RdfXml.ParseType, RdfXml.Namespace, RdfXml.Collection);

            base.WriteList(node);
        }

        protected override void WriteLiteral(ILiteralNode node)
        {
            if (node.DataType != null)
            {
                this.writer.WriteAttributeString(RdfXml.Prefix, RdfXml.Datatype, RdfXml.Namespace, node.DataType.AbsoluteUri);
            }

            if (!string.IsNullOrEmpty(node.Language))
            {
                this.writer.WriteAttributeString(Xml.Prefix, Xml.Lang, XmlSpecsHelper.NamespaceXml, node.Language);
            }

            this.writer.WriteString(node.Value);
        }

        protected override void WriteObjectId(INode node)
        {
            this.WriteId(node, RdfXml.Resource);
        }



        private void WriteId(INode node, string name)
        {
            switch (node)
            {
                case IUriNode uriNode:
                    WriteUriAttribute(uriNode, name);
                    break;

                case IBlankNode blankNode:
                    WriteInternalID(blankNode);
                    break;
            }
        }

        private void WriteUriAttribute(IUriNode node, string name)
        {
            var uri = node.Uri;

            var nodeUriString = uri.AbsoluteUri;

            if (this.Settings.BaseUri != null && this.Settings.BaseUri.IsBaseOf(uri))
            {
                if (this.Settings.BaseUri.ToString().Contains("#"))
                {
                    throw new InvalidOperationException("Can't yet handle base URIs with fragments");
                }

                nodeUriString = this.Settings.BaseUri.MakeRelativeUri(uri).ToString();
            }

            // TODO: @rdf:ID for # URIs?
            this.writer.WriteAttributeString(RdfXml.Prefix, name, RdfXml.Namespace, nodeUriString);
        }

        private void WriteInternalID(IBlankNode node)
        {
            this.writer.WriteAttributeString(RdfXml.Prefix, RdfXml.NodeId, RdfXml.Namespace, node.InternalID);
        }

        private bool TryWritingTypedElement(INode node)
        {
            var rdf_type = this.Graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
            var typeStatement = this.Graph.GetTriplesWithSubjectPredicate(node, rdf_type).FirstOrDefault();
            if (typeStatement == null)
            {
                return false;
            }

            if (!(typeStatement.Object is IUriNode typeNode))
            {
                return false;
            }

            if (!this.TryWritingStartElement(typeNode))
            {
                return false;
            }

            this.Graph.Retract(typeStatement);

            return true;
        }

        private bool TryWritingStartElement(IUriNode node)
        {
            FriendlyRdfXmlWriter.Split(node.Uri, out string nsUri, out string localName);

            if (!XmlSpecsHelper.IsNCName(localName))
            {
                return false;
            }

            this.writer.WriteStartElement(localName, nsUri);

            return true;
        }

        private static void Split(Uri uri, out string nsUri, out string localName)
        {
            var uriString = uri.AbsoluteUri;
            if (uriString.Contains("#"))
            {
                // TODO: Uri.Fragment?
                nsUri = uriString.Substring(0, uriString.LastIndexOf("#") + 1);
            }
            else
            {
                nsUri = uriString.Substring(0, uriString.LastIndexOf("/") + 1);
            }

            localName = new Uri(nsUri).MakeRelativeUri(uri).ToString();

            if (localName.StartsWith("#"))
            {
                localName = localName.Substring(1);
            }
        }
    }
}
