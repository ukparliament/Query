namespace Parliament.Data.Api.FixedQuery.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using VDS.RDF;
    using VDS.RDF.Parsing;

    [TestClass]
    [TestCategory("LocalOnly")]
    public class Comparison
    {
        private static IEnumerable<object[]> FileNames
        {
            get
            {
                return new[] {
                    new[] { @"", @"" }
                };
            }
        }

        [Ignore]
        [TestMethod]
        [DynamicData(nameof(FileNames))]
        public void TwoGraphAreEqual(string expectedFilename, string actualFilename)
        {
            using (var expected = new Graph())
            {
                FileLoader.Load(expected, expectedFilename);

                using (var actual = new Graph())
                {
                    FileLoader.Load(actual, actualFilename);

                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }
}