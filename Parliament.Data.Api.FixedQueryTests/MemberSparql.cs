namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("Sparql")]
    public class MemberSparql : SparqlValidator
    {
        private MemberController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new MemberController();
        }

        [TestMethod()]
        public void MemberCurrentSparql()
        {
            ValidateSparql(() => controller.Current());
        }

        [TestMethod()]
        public void MemberByInitialSparql()
        {
            ValidateSparql(() => controller.ByInitial(null));
        }

        [TestMethod()]
        public void MemberAToZLettersSparql()
        {
            ValidateSparql(() => controller.AToZLetters());
        }

        [TestMethod()]
        public void MemberCurrentByInitialSparql()
        {
            ValidateSparql(() => controller.CurrentByInitial(null));
        }

        [TestMethod()]
        public void MemberCurrentAToZLettersSparql()
        {
            ValidateSparql(() => controller.CurrentAToZLetters());
        }
    }
}