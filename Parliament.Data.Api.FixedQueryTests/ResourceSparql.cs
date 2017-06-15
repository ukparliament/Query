namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("Resource")]
    [TestCategory("Sparql")]
    public class ResourceSparql : SparqlValidator
    {
        private ResourceController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new ResourceController();
        }

        [TestMethod()]
        public void ResourceByIdSparql()
        {
            ValidateSparql(() => controller.ById(string.Empty));
        }
    }
}