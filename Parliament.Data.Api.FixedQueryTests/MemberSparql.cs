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
        private FixedQueryController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new FixedQueryController();
        }

        [TestMethod()]
        public void MemberCurrentSparql()
        {
            ValidateSparql(() => controller.member_current());
        }

        [TestMethod()]
        public void MemberByInitialSparql()
        {
            ValidateSparql(() => controller.member_by_initial(string.Empty));
        }

        [TestMethod()]
        public void MemberAToZLettersSparql()
        {
            ValidateSparql(() => controller.member_a_to_z());
        }

        [TestMethod()]
        public void MemberCurrentByInitialSparql()
        {
            ValidateSparql(() => controller.member_current_by_initial(string.Empty));
        }

        [TestMethod()]
        public void MemberCurrentAToZLettersSparql()
        {
            ValidateSparql(() => controller.member_current_a_to_z());
        }
    }
}