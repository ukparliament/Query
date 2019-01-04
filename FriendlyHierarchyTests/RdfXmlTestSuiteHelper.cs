// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace FriendlyHierarchyTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using VDS.RDF;
    using VDS.RDF.Parsing;
    using VDS.RDF.Parsing.Handlers;

    public static class RdfXmlTestSuiteHelper
    {
        private readonly static Uri baseUri = new Uri("https://www.w3.org/2013/RDFXMLTests/");
        private readonly static IUriNode mf_name;
        private readonly static IGraph graph;
        private readonly static Uri mf = new Uri("http://www.w3.org/2001/sw/DataAccess/tests/test-manifest");
        private readonly static IUriNode mf_action;
        private readonly static IUriNode mf_result;

        static RdfXmlTestSuiteHelper()
        {
            UriLoader.Cache.Clear();
            Options.UriLoaderCaching = false;

            var manifestUri = new Uri(baseUri, "manifest.ttl");
            graph = GraphFromUri(manifestUri);

            mf_name = graph.CreateUriNode(new Uri(mf, "#name"));
            mf_action = graph.CreateUriNode(new Uri(mf, "#action"));
            mf_result = graph.CreateUriNode(new Uri(mf, "#result"));
        }

        public static IEnumerable<object[]> TestCases
        {
            get
            {
                var rdft = new Uri("http://www.w3.org/ns/rdftest");
                var rdfs_a = graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType));
                var rdft_TestXMLEval = graph.CreateUriNode(new Uri(rdft, "#TestXMLEval"));
                var mf_entries = graph.CreateUriNode(new Uri(mf, "#entries"));

                var entryListNode = graph.GetTriplesWithPredicate(mf_entries).Single().Object;
                var entryNodes = graph.GetListItems(entryListNode);

                return entryNodes
                    .Where(entryNode =>
                        graph
                            .GetTriplesWithSubjectPredicate(entryNode, rdfs_a)
                            .WithObject(rdft_TestXMLEval)
                            .Any())
                    .Select(entryNode =>
                        graph
                            .GetTriplesWithSubjectPredicate(entryNode, mf_name)
                            .Select(x => x.Object as ILiteralNode)
                            .Single()
                            .Value)
                    //.Skip(1)
                    //.Take(1)
                    //.Where(name =>
                    //    // name == "rdf-ns-prefix-confusion-test0014" // dotnetrdf doesn't recognize rdf:li?
                    //    // name == "xmlbase-test006" // dotnetrdf doesn't ignore fragment when xml:base has fragment
                    //    // name == "xmlbase-test013" // dotnetrdf doesn't ignore fragment when xml:base has fragment
                    //    // name == "xml-canon-test001" // dotnetrdf doesn't normalize <br></br> to <br/>, so literals are not equal
                    //)
                    .Select(name => new object[] { name });
            }
        }

        public static void GetGraphs(string name, out IGraph action, out IGraph result)
        {
            var nameNode = graph.CreateLiteralNode(name);
            var entryNode = graph.GetTriplesWithPredicateObject(mf_name, nameNode).Single().Subject;
            var actionUri = graph.GetTriplesWithSubjectPredicate(entryNode, mf_action).Select(x => x.Object as IUriNode).Single().Uri;
            var resultUri = graph.GetTriplesWithSubjectPredicate(entryNode, mf_result).Select(x => x.Object as IUriNode).Single().Uri;

            action = GraphFromUri(actionUri);
            result = GraphFromUri(resultUri);
        }

        private static IGraph GraphFromUri(Uri uri)
        {
            var incorrectBaseUri = new Uri(baseUri.AbsoluteUri.Replace(@"https://", "http://"));

            var uriMapping = new Dictionary<Uri, Uri> {
                {
                    incorrectBaseUri,
                    baseUri
                }
            };

            var graph = new Graph();
            var handler = new UriMappingHandler(new GraphHandler(graph), graph, uriMapping);
            var parser = MimeTypesHelper.GetParserByFileExtension(Path.GetExtension(uri.LocalPath));

            graph.BaseUri = uri;
            UriLoader.Load(handler, uri, parser);

            return graph;
        }
    }
}
