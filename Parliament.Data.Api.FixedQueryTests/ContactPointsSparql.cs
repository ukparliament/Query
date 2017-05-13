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
        private ContactPointsController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new ContactPointsController();
        }

        [TestMethod()]
        public void ContactPointsIndexSparql()
        {
            ValidateSparql(() => controller.Index());
        }

        [TestMethod()]
        public void ContactPointsByIdSparql()
        {
            ValidateSparql(() => controller.ById(null));
        }
    }
}