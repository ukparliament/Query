namespace FriendlyHierarchyTests
{
    using FriendlyHierarchy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using VDS.RDF;
    using VDS.RDF.Parsing;
    using VDS.RDF.Writing;

    [TestClass]
    public class RdfXmlTests
    {
        [TestMethod]
        [DynamicData(nameof(RdfXmlTestSuiteHelper.TestCases), typeof(RdfXmlTestSuiteHelper))]
        public void RdfXmlTestSuite(string name)
        {
            RdfXmlTestSuiteHelper.GetGraphs(name, out IGraph action, out IGraph expected);

            // If default readers don't agree they're equal, there's nothing to test
            if (!expected.Equals(action))
            {
                Assert.Inconclusive();
            }

            var actual = new Graph();
            actual.LoadFromString(
                StringWriter.Write(action, new FriendlyRdfXmlWriter()),
                new RdfXmlParser());

            Assert.AreEqual(expected, actual);
        }
    }
}