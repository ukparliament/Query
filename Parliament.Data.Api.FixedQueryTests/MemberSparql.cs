namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("Member")]
    [TestCategory("Sparql")]
    public class MemberSparql : SparqlValidator
    {
        private XController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new XController();
        }

        [TestMethod()]
        public void MemberCurrentSparql()
        {
            ValidateSparql(() => controller.MemberCurrent());
        }

        [TestMethod()]
        public void MemberByInitialSparql()
        {
            ValidateSparql(() => controller.MemberByInitial(string.Empty));
        }

        [TestMethod()]
        public void MemberAToZLettersSparql()
        {
            ValidateSparql(() => controller.MemberAToZ());
        }

        [TestMethod()]
        public void MemberCurrentByInitialSparql()
        {
            ValidateSparql(() => controller.MemberCurrentByInitial(string.Empty));
        }

        [TestMethod()]
        public void MemberCurrentAToZLettersSparql()
        {
            ValidateSparql(() => controller.MemberCurrentAToZ());
        }
    }
}