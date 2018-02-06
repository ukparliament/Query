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