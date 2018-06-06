namespace Parliament.Data.Api.FixedQuery
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Xml;
    using VDS.RDF;
    using VDS.RDF.Nodes;
    using VDS.RDF.Parsing;

    public class HtmlWriter : IRdfWriter
    {
        public event RdfWriterWarning Warning;

        public void Save(IGraph g, string filename) => throw new NotImplementedException();

        public void Save(IGraph g, TextWriter output) => this.Save(g, output, false);

        public void Save(IGraph g, TextWriter output, bool leaveOpen)
        {
            using (var writer = XmlWriter.Create(output, new XmlWriterSettings { Indent = true, CloseOutput = !leaveOpen, OmitXmlDeclaration = true }))
            {
                writer.WriteStartElement("html");
                writer.WriteStartElement("head");
                writer.WriteStartElement("style");
                writer.WriteString(@"
* {
    font-family: sans-serif;
}

body {
    margin: 0;
}

table {
    width: 100%;
    border-collapse: collapse;
}

td {
    b order: 1px solid;
    padding: 20px;
    vertical-align: top;
}

a {
    font-family: monospace;
    text-decoration: none;
    position: sticky;
    left: 20px;
    top: 20px;
}

a:hover {
    text-decoration: underline;
}
");
                writer.WriteEndElement(); // style
                writer.WriteEndElement(); // head
                writer.WriteStartElement("body");
                writer.WriteStartElement("table");
                writer.WriteStartElement("thead");
                writer.WriteStartElement("tr");
                writer.WriteStartElement("th");
                writer.WriteString("subject");
                writer.WriteEndElement(); // th
                writer.WriteStartElement("th");
                writer.WriteString("predicate");
                writer.WriteEndElement(); // th
                writer.WriteStartElement("th");
                writer.WriteString("object");
                writer.WriteEndElement(); // th
                writer.WriteEndElement(); // tr
                writer.WriteEndElement(); // thead

                writer.WriteStartElement("tbody");
                foreach (var subject in g.Triples.SubjectNodes)
                {
                    writer.WriteStartElement("tr");
                    writer.WriteAttributeString("style", "border-top: 1px solid");

                    var subjectTriples = g.GetTriplesWithSubject(subject);

                    writer.WriteStartElement("td");
                    var subjectTripleCount = subjectTriples.Count();
                    if (subjectTripleCount > 1)
                    {
                        writer.WriteAttributeString("rowspan", subjectTripleCount.ToString());
                    }
                    WriteNode(writer, subject);
                    writer.WriteEndElement(); // td

                    var predicates = subjectTriples.Select(t => t.Predicate).Distinct();

                    var fp = true;
                    foreach (var predicate in predicates)
                    {
                        if (!fp)
                        {
                            writer.WriteStartElement("tr");
                            writer.WriteAttributeString("style", "border-top: 1px solid");
                        }

                        var predicateTriples = subjectTriples.WithPredicate(predicate);

                        writer.WriteStartElement("td");
                        var predicateTripleCount = predicateTriples.Count();
                        if (predicateTripleCount > 1)
                        {
                            writer.WriteAttributeString("rowspan", predicateTripleCount.ToString());
                        }
                        WriteNode(writer, predicate);
                        writer.WriteEndElement(); // td

                        var fo = true;
                        foreach (var o in predicateTriples)
                        {
                            if (!fo)
                            {
                                writer.WriteStartElement("tr");
                            }

                            writer.WriteStartElement("td");
                            WriteNode(writer, o.Object);
                            writer.WriteEndElement(); // td

                            writer.WriteEndElement(); // tr

                            fo = false;
                        }
                        fp = false;
                    }
                }

                writer.WriteEndElement(); // tbody
                writer.WriteEndElement(); // table
                writer.WriteEndElement(); // body
                writer.WriteEndElement(); // html
            }
        }

        private static void WriteNode(XmlWriter writer, INode node)
        {
            var valuedNode = node.AsValuedNode();
            switch (valuedNode)
            {
                case IUriNode uriNode:
                    var x = uriNode.Uri.AbsoluteUri;
                    x = x.Replace(Global.SchemaUri.AbsoluteUri, string.Empty);
                    x = x.Replace(Global.InstanceUri.AbsoluteUri, string.Empty);
                    x = x.Replace(RdfSpecsHelper.RdfType, "a");
                    x = x.Replace("http://example.com/", string.Empty);
                    x = x.Replace("http://www.w3.org/2000/01/rdf-schema#", string.Empty);
                    x = x.Replace("http://www.w3.org/2002/07/owl#", string.Empty);
                    x = x.Replace("http://www.w3.org/1999/02/22-rdf-syntax-ns#", string.Empty);

                    writer.WriteStartElement("a");

                    writer.WriteAttributeString("href", "/resource?uri=" + WebUtility.UrlEncode(uriNode.Uri.AbsoluteUri));
                    writer.WriteString(x);
                    writer.WriteEndElement(); // a
                    break;

                case DateTimeNode dateTimeNode:
                    writer.WriteStartElement("time");
                    writer.WriteString(dateTimeNode.Value);
                    writer.WriteEndElement(); // time
                    break;

                default:
                    writer.WriteString(node.ToString());
                    break;
            }
        }
    }
}