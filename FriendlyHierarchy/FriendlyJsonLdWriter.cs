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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;
    using System.Linq;
    using VDS.RDF;
    using VDS.RDF.Nodes;
    using VDS.RDF.Parsing;

    internal class Mapping { public string Prefix; public Uri Namespace; }

    // TODO: Support IStoreWriter?
    public class FriendlyJsonLdWriter : SpanningAlgorithm, IRdfWriter
    {
        private static readonly IUriNode rdf_type = new NodeFactory().CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
        private JsonTextWriter writer;

        private FriendlyJsonLDWriterSettings Settings
        {
            get
            {
                return base.settings as FriendlyJsonLDWriterSettings;
            }
        }

        public FriendlyJsonLdWriter() : this(new FriendlyJsonLDWriterSettings())
        {
        }

        public FriendlyJsonLdWriter(FriendlyJsonLDWriterSettings settings) : base(settings)
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
            this.writer = new JsonTextWriter(output) { Formatting = Formatting.Indented };
            this.Initialize(g);
            this.WriteRdf();
        }
        #endregion

        protected override void WriteRdf()
        {
            this.writer.WriteStartObject();

            this.WriteContext();

            this.writer.WritePropertyName(JsonLd.Graph);
            this.writer.WriteStartArray();

            base.WriteRdf();

            this.writer.WriteEnd();
            this.writer.WriteEnd();
        }

        protected override void WriteSubject(INode node)
        {
            this.writer.WriteStartObject();

            base.WriteSubject(node);

            this.writer.WriteEnd();
        }

        protected override void WriteSubjectId(INode node)
        {
            this.writer.WritePropertyName(JsonLd.Id);

            this.WriteIdValue(node, this.Settings.BaseUri);
        }

        protected override void WritePredicateGroup(IGrouping<IUriNode, Triple> statements)
        {
            this.writer.WritePropertyName(ConvertPredicate(statements.Key));

            var writeArray = this.Settings.AlwaysUseArrays || statements.Count() > 1;

            if (writeArray)
            {
                this.writer.WriteStartArray();
            }

            base.WritePredicateGroup(statements);

            if (writeArray)
            {
                this.writer.WriteEnd();
            }
        }

        protected override void WritePredicate(Triple statement)
        {
            if (this.Settings.LiftTypes && statement.Predicate.Equals(rdf_type))
            {
                this.WriteIdValue(statement.Object, this.Settings.VocabularyUri);
            }
            else
            {
                base.WritePredicate(statement);
            }
        }

        protected override void WriteList(INode node)
        {
            this.writer.WriteStartObject();
            this.writer.WritePropertyName(JsonLd.List);
            this.writer.WriteStartArray();

            base.WriteList(node);

            this.writer.WriteEnd();
            this.writer.WriteEnd();
        }

        protected override void WriteLiteral(ILiteralNode node)
        {
            var token = null as JToken;

            if (!string.IsNullOrEmpty(node.Language))
            {
                token = new JObject(
                    new JProperty(
                        JsonLd.Language,
                        node.Language),
                    new JProperty(
                        JsonLd.Value,
                        node.Value));
            }
            else
            {
                var vn = node.AsValuedNode();

                switch (vn)
                {
                    case StringNode valueNode:
                        token = new JValue(valueNode.AsString());
                        break;

                    case BooleanNode valueNode:
                        token = new JValue(valueNode.AsBoolean());
                        break;

                    case LongNode valueNode:
                        token = new JValue(valueNode.AsInteger());
                        break;

                    case DoubleNode valueNode:
                        token = new JValue(valueNode.AsDouble());
                        break;
                }

                if (token == null || vn is StringNode stringNode && node.DataType != null && vn.EffectiveType != XmlSpecsHelper.XmlSchemaDataTypeString)
                {
                    token = new JObject(
                        new JProperty(
                            JsonLd.Type,
                            this.MakeRelative(node.DataType, this.Settings.VocabularyUri)),
                        new JProperty(
                            JsonLd.Value,
                            node.Value));
                }
            }

            if (this.Settings.AlwaysUseStructuredLiterals && token is JValue)
            {
                token = new JObject(
                    new JProperty(
                        JsonLd.Value,
                        token));
            }

            this.writer.WriteToken(token.CreateReader());
        }

        protected override void WriteObjectId(INode node)
        {
            this.writer.WriteStartObject();

            this.WriteSubjectId(node);

            this.writer.WriteEnd();
        }


        private void WriteContext()
        {
            if (this.Settings.CompactUris)
            {
                var mappings =
                    new Mapping
                    {
                        Prefix = JsonLd.Base,
                        Namespace = this.Settings.BaseUri
                    }
                    .AsEnumerable()
                    .Where(mapping => this.Settings.BaseUri != null);

                mappings = mappings.Union(
                    new Mapping
                    {
                        Prefix = JsonLd.Vocab,
                        Namespace = this.Settings.VocabularyUri
                    }
                    .AsEnumerable()
                    .Where(mapping => this.Settings.VocabularyUri != null));

                mappings = mappings.Union(
                    Graph
                    .NamespaceMap
                    .Prefixes
                    .Select(prefix =>
                        new Mapping
                        {
                            Prefix = prefix,
                            Namespace = this.Graph.NamespaceMap.GetNamespaceUri(prefix)
                        })
                    .Where(mapping =>
                        !new[] {
                            this.Settings.BaseUri,
                            this.Settings.VocabularyUri
                        }
                        .Contains(mapping.Namespace))
                );

                if (mappings.Any())
                {
                    this.writer.WritePropertyName(JsonLd.Context);
                    this.writer.WriteStartObject();

                    foreach (var item in mappings)
                    {
                        this.writer.WritePropertyName(item.Prefix);
                        this.writer.WriteValue(item.Namespace);
                    }

                    this.writer.WriteEnd();
                }
            }
        }

        private void WriteIdValue(INode node, Uri baseUri)
        {
            switch (node)
            {
                case IUriNode uriNode:
                    var uri = uriNode.Uri;
                    var nodeUriString = this.MakeRelative(uri, baseUri);

                    this.writer.WriteValue(nodeUriString);

                    break;

                case IBlankNode blankNode:
                    this.writer.WriteValue(blankNode.ToString());

                    break;
            }
        }

        private string MakeRelative(Uri uri, Uri @base)
        {
            if (this.Settings.CompactUris)
            {
                if (@base != null && @base.IsBaseOf(uri))
                {
                    uri = @base.MakeRelativeUri(uri);
                }
                else if (this.Graph.NamespaceMap.ReduceToQName(uri.ToString(), out string qname))
                {
                    return qname;
                }
            }

            return uri.ToString();
        }

        private string ConvertPredicate(IUriNode predicate)
        {
            if (this.Settings.LiftTypes && predicate.Equals(rdf_type))
            {
                return JsonLd.Type;
            }
            else
            {
                return this.MakeRelative(predicate.Uri, this.Settings.VocabularyUri);
            }
        }
    }
}
