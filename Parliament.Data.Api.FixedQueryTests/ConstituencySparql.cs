namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("Constituency")]
    [TestCategory("Sparql")]
    public class ConstituencySparql : SparqlValidator
    {
        private ConstituencyController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new ConstituencyController();
        }

        [TestMethod()]
        public void ConstituencyByIdSparql()
        {
            ValidateSparql(() => controller.ById(null));
        }

        [TestMethod()]
        public void ConstituencyByInitialSparql()
        {
            ValidateSparql(() => controller.ByInitial(null));
        }

        [TestMethod()]
        public void ConstituencyCurrentSparql()
        {
            ValidateSparql(() => controller.Current());
        }

        [TestMethod()]
        public void ConstituencyLookupSparql()
        {
            ValidateSparql(() => controller.Lookup(null, null));
        }

        [TestMethod()]
        public void ConstituencyByLettersSparql()
        {
            ValidateSparql(() => controller.ByLetters(null));
        }

        [TestMethod()]
        public void ConstituencyAToZLettersSparql()
        {
            ValidateSparql(() => controller.AToZLetters());
        }

        [TestMethod()]
        public void ConstituencyCurrentByLettersSparql()
        {
            ValidateSparql(() => controller.CurrentByLetters(null));
        }

        [TestMethod()]
        public void ConstituencyCurrentAToZLettersSparql()
        {
            ValidateSparql(() => controller.CurrentAToZLetters());
        }

        [TestMethod()]
        public void ConstituencyIndexSparql()
        {
            ValidateSparql(() => controller.Index());
        }

        [TestMethod()]
        public void ConstituencyMembersSparql()
        {
            ValidateSparql(() => controller.Members(null));
        }

        [TestMethod()]
        public void ConstituencyCurrentMembersSparql()
        {
            ValidateSparql(() => controller.CurrentMembers(null));
        }

        [TestMethod()]
        public void ConstituencyContactPointSparql()
        {
            ValidateSparql(() => controller.ContactPoint(null));
        }

        [TestMethod()]
        public void ConstituencyLookupByPostcodeSparql()
        {
            ValidateSparql(() => controller.LookupByPostcode(null));
        }
    }
}