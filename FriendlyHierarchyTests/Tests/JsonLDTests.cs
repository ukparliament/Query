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
    using FriendlyHierarchy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using VDS.RDF;
    using VDS.RDF.Parsing;
    using VDS.RDF.Parsing.Handlers;
    using VDS.RDF.Writing;

    [TestClass]
    [TestCategory("LocalOnly")]
    [TestCategory("FriendlyHierarchy")]
    public class JsonLDTests
    {
        [TestMethod]
        [DynamicData(nameof(RdfXmlTestSuiteHelper.TestCases), typeof(RdfXmlTestSuiteHelper))]
        public void JsonLDTestSuite(string name)
        {
            RdfXmlTestSuiteHelper.GetGraphs(name, out IGraph action, out IGraph expected);

            if (!expected.Equals(action))
            {
                Assert.Inconclusive();
            }

            var actual = new TripleStore();
            actual.LoadFromString(
                StringWriter.Write(action, new FriendlyJsonLdWriter()),
                new JsonLdParser());

            if (!(actual.IsEmpty && expected.IsEmpty))
            {
                Assert.AreEqual(
                    Normalize(expected),
                    Normalize(actual.Graphs.Single()));
            }
        }

        private static Graph Normalize(IGraph original)
        {
            var normalized = new Graph();

            new StripStringHandler(new NormalizeDateHandler(new GraphHandler(normalized))).Apply(original);

            return normalized;
        }

    }
}