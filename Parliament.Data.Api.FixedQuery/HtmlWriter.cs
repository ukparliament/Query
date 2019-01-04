// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
                var d = a.ToDictionary(r => r["subject"] as IUriNode, r => r["label"].AsValuedNode().AsString());





                writer.WriteStartElement("html");
                writer.WriteStartElement("head");
                writer.WriteRaw(@"
<link rel=""stylesheet"" href=""https://unpkg.com/leaflet@1.3.1/dist/leaflet.css"" integrity=""sha512-Rksm5RenBEKSKFjgI3a41vrjkw4EVPlJ3+OiI65vTjIdo9brlAacEuKOiQ5OFh7cOI1bkDwLqdLw3Zg0cRJAAQ=="" crossorigin="""" />
<script src=""https://unpkg.com/leaflet@1.3.1/dist/leaflet.js"" integrity=""sha512-/Nsx9X4HebavoBvEBuyp3I7od5tA0UzAxs+j83KgC8PU0kgB4XiK4Lfe4y4cgBtaRJQEIFCW+oC506aPT2L1zw=="" crossorigin=""""></script>
<script src=""https://api.tiles.mapbox.com/mapbox.js/plugins/leaflet-omnivore/v0.3.1/leaflet-omnivore.min.js""></script>
<script>
    window.addEventListener(""load"", onLoad);

    function onLoad() {
        document.querySelectorAll(""data.map"").forEach(createMap);
    }

    function createMap(mapElement) {
        const map = L.map(mapElement);

        L.tileLayer(""https://api.tiles.mapbox.com/v4/{id}/{z}/{x}/{y}.png?access_token={accessToken}"", {
            id: ""mapbox.streets"",
            accessToken: ""pk.eyJ1IjoiaHVudHAiLCJhIjoiY2l6cXY3NjZpMDAxZzJybzF0aDBvdHRlZCJ9.k1zL5uDY7eUvuSiw3Rdrkw""
        }).addTo(map);

        const geometryLayer = omnivore.wkt.parse(mapElement.value);
        geometryLayer.addTo(map);
        map.fitBounds(geometryLayer.getBounds());
    }
</script>
<style>
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
        text-align: left;
        padding-left: 20px;
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

    data.map {
        height: 180px;
        display: block
    }

    a {
        text-decoration: none;
    }

    a:hover {
        text-decoration: underline;
    }
</style>
<script type=""text/javascript"">
  var appInsights=window.appInsights||function(a){
    function b(a){c[a]=function(){var b=arguments;c.queue.push(function(){c[a].apply(c,b)})}}var c={config:a},d=document,e=window;setTimeout(function(){var b=d.createElement(""script"");b.src=a.url||""https://az416426.vo.msecnd.net/scripts/a/ai.0.js"",d.getElementsByTagName(""script"")[0].parentNode.appendChild(b)});try{c.cookie=d.cookie}catch(a){}c.queue=[];for(var f=[""Event"",""Exception"",""Metric"",""PageView"",""Trace"",""Dependency""];f.length;)b(""track""+f.pop());if(b(""setAuthenticatedUserContext""),b(""clearAuthenticatedUserContext""),b(""startTrackEvent""),b(""stopTrackEvent""),b(""startTrackPage""),b(""stopTrackPage""),b(""flush""),!a.disableExceptionTracking){f=""onerror"",b(""_""+f);var g=e[f];e[f]=function(a,b,d,e,h){var i=g&&g(a,b,d,e,h);return!0!==i&&c[""_""+f](a,b,d,e,h),i}}return c
    }({
        instrumentationKey:""c0960a0f-30ad-4a9a-b508-14c6a4f61179"",
        cookieDomain:"".parliament.uk""
    });
    
  window.appInsights=appInsights,appInsights.queue&&0===appInsights.queue.length&&appInsights.trackPageView();
</script>
");
                writer.WriteEndElement(); // head
                writer.WriteStartElement("body");
                writer.WriteStartElement("table");
                HtmlWriter.WriteTHead(writer);
                HtmlWriter.WriteTBody(g, writer, d);
                writer.WriteEndElement(); // table
                writer.WriteEndElement(); // body
                writer.WriteEndElement(); // html
            }
        }

        private static void WriteTBody(IGraph g, XmlWriter writer, Dictionary<IUriNode, string> labelMapping)
        {
            writer.WriteStartElement("tbody");

            var subjects = g.Triples.SubjectNodes;
            foreach (var subject in subjects)
            {
                var writeSubject = true;

                var subjectTriples = g.GetTriplesWithSubject(subject);
                var predicates = subjectTriples.Select(t => t.Predicate).Distinct();
                foreach (var predicate in predicates)
                {
                    var writePredicate = true;

                    var predicateTriples = subjectTriples.WithPredicate(predicate);
                    foreach (var t in subjectTriples.WithPredicate(predicate))
                    {
                        writer.WriteStartElement("tr");

                        if (writeSubject || writePredicate)
                        {
                            writer.WriteAttributeString("class", "divider");
                        }

                        if (writeSubject)
                        {
                            writeSubject = false;

                            HtmlWriter.WriteCell(writer, t, labelMapping, TripleSegment.Subject, subjectTriples.Count());
                        }

                        if (writePredicate)
                        {
                            writePredicate = false;

                            HtmlWriter.WriteCell(writer, t, labelMapping, TripleSegment.Predicate, predicateTriples.Count());
                        }

                        HtmlWriter.WriteCell(writer, t, labelMapping, TripleSegment.Object, 0);

                        writer.WriteEndElement(); // tr
                    }
                }
            }

            writer.WriteEndElement(); // tbody
        }

        private static void WriteCell(XmlWriter writer, Triple t, Dictionary<IUriNode, string> labelMapping, TripleSegment segment, int tripleCount)
        {
            writer.WriteStartElement("td");

            if (tripleCount > 1)
            {
                writer.WriteAttributeString("rowspan", tripleCount.ToString());
            }

            HtmlWriter.WriteNode(writer, t, labelMapping, segment);
            writer.WriteEndElement(); // td
        }

        private static void WriteTHead(XmlWriter writer)
        {
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
        }

        private static void WriteNode(XmlWriter writer, Triple triple, Dictionary<IUriNode, string> labelMapping, TripleSegment segment)
        {
            var node = HtmlWriter.GetNode(triple, segment);

            if (node is IUriNode uriNode)
            {
                var uri = uriNode.Uri.AbsoluteUri;

                if (segment == TripleSegment.Object && HtmlWriter.IsPhoto(triple.Predicate))
                {
                    var id = Global.InstanceUri.MakeRelativeUri(uriNode.Uri);

                    writer.WriteStartElement("a");
                    writer.WriteAttributeString("href", "resource?uri=" + WebUtility.UrlEncode(uri));
                    writer.WriteStartElement("data");
                    writer.WriteAttributeString("value", uri);
                    writer.WriteStartElement("img");
                    writer.WriteAttributeString("src", $"https://api.parliament.uk/photo/{id}.jpeg?crop=MCU_3:2&width=260&quality=80");
                    writer.WriteEndElement(); // data
                    writer.WriteEndElement(); // img
                    writer.WriteEndElement(); // a

                    return;
                }

                if (!labelMapping.TryGetValue(uriNode, out string label))
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

                writer.WriteStartElement("div");
                writer.WriteStartElement("a");
                writer.WriteAttributeString("href", "resource?uri=" + WebUtility.UrlEncode(uri));

                writer.WriteStartElement("data");
                writer.WriteAttributeString("value", uri);
                writer.WriteString(label);
                writer.WriteEndElement(); // data

                writer.WriteEndElement(); // a
                writer.WriteEndElement(); // div

                return;
            }

            if (node is IBlankNode blankNode)
            {
                if (segment == TripleSegment.Subject)
                {
                    writer.WriteStartElement("div");
                    writer.WriteStartElement("a");
                    writer.WriteAttributeString("name", blankNode.InternalID);
                    writer.WriteFullEndElement(); // a

                    writer.WriteString(blankNode.InternalID);
                    writer.WriteEndElement(); // div

                    return;
                }

                if (segment == TripleSegment.Object)
                {
                    writer.WriteStartElement("a");
                    writer.WriteAttributeString("href", "#" + blankNode.InternalID);
                    writer.WriteString(blankNode.InternalID);
                    writer.WriteEndElement(); // a

                    return;
                }
            }

            if (node is ILiteralNode literalNode)
            {
                var datatype = literalNode.DataType?.AbsoluteUri;

                if (datatype == XmlSpecsHelper.XmlSchemaDataTypeDate)
                {
                    if (DateTimeOffset.TryParse(literalNode.Value, out DateTimeOffset dto))
                    {
                        writer.WriteStartElement("time");
                        writer.WriteString(dto.ToString("yyyy-MM-dd"));
                        writer.WriteEndElement(); // time

                        return;
                    }
                }

                if (datatype == "http://www.opengis.net/ont/geosparql#wktLiteral")
                {
                    if (HtmlWriter.IsGeography(triple.Predicate))
                    {
                        writer.WriteStartElement("data");
                        writer.WriteAttributeString("class", "map");
                        writer.WriteAttributeString("value", literalNode.Value);
                        writer.WriteEndElement(); // data

                        return;
                    }
                }

                writer.WriteString(literalNode.Value);

                return;
            }

            writer.WriteString(node.ToString());
        }

        private static bool IsPhoto(INode predicate)
        {
            var factory = new NodeFactory();
            var photoPredicates = new[] { factory.CreateUriNode(new Uri("https://id.parliament.uk/schema/memberHasMemberImage")) };

            return photoPredicates.Contains(predicate);
        }

        private static bool IsGeography(INode predicate)
        {
            var factory = new NodeFactory();
            var geographyPredicates = new[] { factory.CreateUriNode(new Uri("https://id.parliament.uk/schema/constituencyAreaExtent")) };

            return geographyPredicates.Contains(predicate);
        }

        private static INode GetNode(Triple triple, TripleSegment segment)
        {
            switch (segment)
            {
                case TripleSegment.Subject:
                    return triple.Subject;

                case TripleSegment.Predicate:
                    return triple.Predicate;

                case TripleSegment.Object:
                    return triple.Object;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}