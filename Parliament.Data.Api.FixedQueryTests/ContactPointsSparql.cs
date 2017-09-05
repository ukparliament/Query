namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("ContactPoints")]
    [TestCategory("Sparql")]
    public class ContactPointsSparql : SparqlValidator
    {
        private FixedQueryController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new FixedQueryController();
        }

        [TestMethod()]
        public void ContactPointIndexSparql()
        {
            ValidateSparql(() => controller.contact_point_index());
        }

        [TestMethod()]
        public void ContactPointByIdSparql()
        {
            ValidateSparql(() => controller.contact_point_by_id(string.Empty));
        }
    }
}