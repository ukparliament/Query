namespace FriendlyHierarchyTests
{
    using FriendlyHierarchy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using VDS.RDF;
    using VDS.RDF.Parsing;
    using VDS.RDF.Writing;

    [TestClass]
    public class UsageBased
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            FixedQueryHelper.Properties = context.Properties;
        }

        [TestMethod]
        [DynamicData(nameof(FixedQueryHelper.Specific), typeof(FixedQueryHelper))]
        public void XmlSpecific(string endpoint)
        {
            FixedQuery(endpoint);
        }

        [TestMethod]
        [DynamicData(nameof(FixedQueryHelper.All), typeof(FixedQueryHelper))]
        public void XmlAll(string endpoint)
        {
            FixedQuery(endpoint);
        }

        private void FixedQuery(string endpoint)
        {
            var expected = FixedQueryHelper.x(endpoint);

            using (var actual = new Graph())
            {
                actual.LoadFromString(
                    StringWriter.Write(expected, new FriendlyRdfXmlWriter()),
                    new RdfXmlParser());

                Assert.AreEqual(
                    expected,
                    actual);
            }

        }
    }
}
