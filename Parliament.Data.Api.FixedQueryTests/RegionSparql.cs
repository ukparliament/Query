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
        public void RegionByIdSparql()
        {
            ValidateSparql(() => controller.region_by_id(string.Empty));
        }

        [TestMethod()]
        public void RegionConstituenciesSparql()
        {
            ValidateSparql(() => controller.region_constituencies(string.Empty));
        }

        [TestMethod()]
        public void RegionConstituenciesAtoZSparql()
        {
            ValidateSparql(() => controller.region_constituencies_a_to_z(string.Empty));
        }

        [TestMethod()]
        public void RegionConstituenciesByInitialSparql()
        {
            ValidateSparql(() => controller.region_constituencies_by_initial(string.Empty, string.Empty));
        }
    }
}
