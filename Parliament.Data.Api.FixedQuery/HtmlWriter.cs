namespace Parliament.Data.Api.FixedQuery
{
    using Parliament.Data.Api.FixedQuery.Controllers;
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
    using VDS.RDF.Query;
    using VDS.RDF.Writing;

    public class HtmlWriter : IRdfWriter
    {
        public event RdfWriterWarning Warning;

        public void Save(IGraph g, string filename) => throw new NotImplementedException();

        public void Save(IGraph g, TextWriter output) => this.Save(g, output, false);

        public void Save(IGraph g, TextWriter output, bool leaveOpen)
        {
            using (var writer = XmlWriter.Create(output, new XmlWriterSettings { Indent = true, CloseOutput = !leaveOpen, OmitXmlDeclaration = true }))
            {
                var n = g.Nodes.Union(g.Triples.PredicateNodes).UriNodes();
                var n2 = string.Join(",", n.Select(nn => nn.Uri.AbsoluteUri));
                var a = FixedQueryController.ExecuteNamedSparql("label", new Dictionary<string, string> { { "id", n2 } }) as SparqlResultSet;
                var d = a.ToDictionary(r => r["subject"].ToString(), r => r["label"].ToString());





                writer.WriteStartElement("html");
                writer.WriteStartElement("head");
                writer.WriteStartElement("style");
                writer.WriteString(@"
body {
    margin: 0;
    font-family: sans-serif;
}

table {
    width: 100%;
    border-collapse: collapse;
}

thead th {
    height: 50px;
    position: sticky;
    top: 0;
    z-index: 1;
    background-color: black;
    color: white;
}

tr.divider {
    border-top: 1px solid;
}

tr.divider:nth-child(1) {
    border-top: none;
}

td {
    padding: 20px;
    vertical-align: top;
}

div {
    position: sticky;
    left: 20px;
    top: 70px;
}

data {
    font-family: monospace;
}

a {
    text-decoration: none;
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
                writer.WriteString("Subject");
                writer.WriteEndElement(); // th
                writer.WriteStartElement("th");
                writer.WriteString("Predicate");
                writer.WriteEndElement(); // th
                writer.WriteStartElement("th");
                writer.WriteString("Object");
                writer.WriteEndElement(); // th
                writer.WriteEndElement(); // tr
                writer.WriteEndElement(); // thead

                writer.WriteStartElement("tbody");

                foreach (var subject in g.Triples.SubjectNodes)
                {
                    writer.WriteStartElement("tr");
                    writer.WriteAttributeString("class", "divider");

                    var subjectTriples = g.GetTriplesWithSubject(subject);

                    writer.WriteStartElement("td");
                    var subjectTripleCount = subjectTriples.Count();
                    if (subjectTripleCount > 1)
                    {
                        writer.WriteAttributeString("rowspan", subjectTripleCount.ToString());
                    }
                    WriteNode(writer, subject, d, TripleSegment.Subject);
                    writer.WriteEndElement(); // td

                    var predicates = subjectTriples.Select(t => t.Predicate).Distinct();

                    var isFirstPredicate = true;
                    foreach (var predicate in predicates)
                    {
                        if (!isFirstPredicate)
                        {
                            writer.WriteStartElement("tr");
                            writer.WriteAttributeString("class", "divider");
                        }

                        var predicateTriples = subjectTriples.WithPredicate(predicate);

                        writer.WriteStartElement("td");
                        var predicateTripleCount = predicateTriples.Count();
                        if (predicateTripleCount > 1)
                        {
                            writer.WriteAttributeString("rowspan", predicateTripleCount.ToString());
                        }
                        WriteNode(writer, predicate, d, TripleSegment.Predicate);
                        writer.WriteEndElement(); // td

                        var isFirstObject = true;
                        foreach (var o in predicateTriples)
                        {
                            if (!isFirstObject)
                            {
                                writer.WriteStartElement("tr");
                            }

                            writer.WriteStartElement("td");
                            WriteNode(writer, o.Object, d, TripleSegment.Object);
                            writer.WriteEndElement(); // td

                            writer.WriteEndElement(); // tr

                            isFirstObject = false;
                        }

                        isFirstPredicate = false;
                    }
                }

                writer.WriteEndElement(); // tbody
                writer.WriteEndElement(); // table
                writer.WriteEndElement(); // body
                writer.WriteEndElement(); // html
            }
        }

        private static void WriteNode(XmlWriter writer, INode node, Dictionary<string, string> map, TripleSegment segment)
        {
            writer.WriteStartElement("div");

            switch (node)
            {
                case IUriNode uriNode:
                    var uri = uriNode.Uri.AbsoluteUri;

                    if (!map.TryGetValue(uri, out string label))
                    {
                        label = uri
                            .Replace(Global.SchemaUri.AbsoluteUri, string.Empty)
                            .Replace(Global.InstanceUri.AbsoluteUri, string.Empty)
                            .Replace(RdfSpecsHelper.RdfType, "a")
                            .Replace("http://example.com/", string.Empty)
                            .Replace("http://www.w3.org/2000/01/rdf-schema#", string.Empty)
                            .Replace("http://www.w3.org/2002/07/owl#", string.Empty)
                            .Replace("http://www.w3.org/1999/02/22-rdf-syntax-ns#", string.Empty);
                    }


                    writer.WriteStartElement("a");
                    writer.WriteAttributeString("href", "/resource?uri=" + WebUtility.UrlEncode(uriNode.Uri.AbsoluteUri));

                    writer.WriteStartElement("data");
                    writer.WriteAttributeString("value", uriNode.Uri.AbsoluteUri);
                    writer.WriteString(label);
                    writer.WriteEndElement(); // a

                    writer.WriteEndElement(); // a
                    break;

                case IBlankNode blankNode:
                    switch (segment)
                    {
                        case TripleSegment.Subject:
                            writer.WriteStartElement("a");
                            writer.WriteAttributeString("name", blankNode.InternalID);
                            writer.WriteFullEndElement(); // a

                            writer.WriteString(blankNode.InternalID);
                            break;

                        case TripleSegment.Object:
                            writer.WriteStartElement("a");
                            writer.WriteAttributeString("href", "#" + blankNode.InternalID);
                            writer.WriteString(blankNode.InternalID);
                            writer.WriteEndElement(); // a

                            break;

                        default:
                            break;
                    }
                    break;

                case ILiteralNode literalNode:
                    switch (literalNode.DataType?.AbsoluteUri)
                    {
                        case XmlSpecsHelper.XmlSchemaDataTypeDate:
                            if (DateTimeOffset.TryParse(literalNode.Value, out DateTimeOffset dto))
                            {
                                writer.WriteStartElement("time");
                                writer.WriteString(dto.ToString("yyyy-MM-dd"));
                                writer.WriteEndElement(); // time
                            }
                            else
                            {
                                writer.WriteString(literalNode.Value);
                            }

                            break;

                        default:
                            writer.WriteString(literalNode.Value);
                            break;
                    }
                    break;

                default:
                    break;
            }
            writer.WriteEndElement(); // div
        }
    }
}