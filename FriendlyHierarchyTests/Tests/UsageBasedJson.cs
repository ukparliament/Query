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
    public class UsageBasedJson
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            FixedQueryHelper.Properties = context.Properties;
        }

        [TestMethod]
        [DynamicData(nameof(FixedQueryHelper.Specific), typeof(FixedQueryHelper))]
        public void JsonSpecific(string endpoint)
        {
            FixedQuery(endpoint);
        }

        [TestMethod]
        [DynamicData(nameof(FixedQueryHelper.All), typeof(FixedQueryHelper))]
        public void JsonAll(string endpoint)
        {
            FixedQuery(endpoint);
        }

        private void FixedQuery(string endpoint)
        {
            using (var originalStore = new TripleStore())
            {
                var expected = FixedQueryHelper.x(endpoint);

                if (expected == null)
                {
                    Assert.Inconclusive("Empty graph");
                }

                using (var actual = new TripleStore())
                {
                    actual.LoadFromString(
                        StringWriter.Write(expected, new FriendlyJsonLdWriter()),
                        new JsonLdParser());


                    if (!(actual.IsEmpty && expected.IsEmpty))
                    {
                        Assert.AreEqual(
                            Normalize(expected),
                            Normalize(actual.Graphs.Single()));
                    }
                }
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
