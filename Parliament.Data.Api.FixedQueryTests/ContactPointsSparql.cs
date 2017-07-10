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
        private XController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new XController();
        }

        [TestMethod()]
        public void ContactPointsIndexSparql()
        {
            ValidateSparql(() => controller.ContactPointIndex());
        }

        [TestMethod()]
        public void ContactPointsByIdSparql()
        {
            ValidateSparql(() => controller.ContactPointById(string.Empty));
        }
    }
}