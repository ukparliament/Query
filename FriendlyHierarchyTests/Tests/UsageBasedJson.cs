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
