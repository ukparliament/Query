namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("Region")]
    [TestCategory("Sparql")]
    public class RegionSparql : SparqlValidator
    {
        private FixedQueryController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new FixedQueryController();
        }

        [TestMethod()]
        public void RegionIndexSparql()
        {
            ValidateSparql(() => controller.region_index());
        }

        [TestMethod()]
        public void RegionConstituenciesSparql()
        {
            ValidateSparql(() => controller.region_constituencies(string.Empty));
        }
    }
}