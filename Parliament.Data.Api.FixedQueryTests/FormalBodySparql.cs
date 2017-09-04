namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("FormalBody")]
    [TestCategory("Sparql")]
    public class FormalBodySparql : SparqlValidator
    {
        private FixedQueryController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new FixedQueryController();
        }

        [TestMethod()]
        public void FormalBodyIndexSparql()
        {
            ValidateSparql(() => controller.formal_body_index());
        }

        [TestMethod()]
        public void FormalBodyByIdSparql()
        {
            ValidateSparql(() => controller.formal_body_by_id(string.Empty));
        }

        [TestMethod()]
        public void FormalBodyMembershipSparql()
        {
            ValidateSparql(() => controller.formal_body_membership(string.Empty));
        }
    }
}