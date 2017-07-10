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
        private XController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new XController();
        }

        [TestMethod()]
        public void ConstituencyByIdSparql()
        {
            ValidateSparql(() => controller.ConstituencyByID(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyMapSparql()
        {
            ValidateSparql(() => controller.ConstituencyMap(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyByInitialSparql()
        {
            ValidateSparql(() => controller.ConstituencyByInitial(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyCurrentSparql()
        {
            ValidateSparql(() => controller.ConstituencyCurrent());
        }

        [TestMethod()]
        public void ConstituencyLookupSparql()
        {
            ValidateSparql(() => controller.ConstituencyLookup(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ConstituencyByLettersSparql()
        {
            ValidateSparql(() => controller.ConstituencyByLetters(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyAToZLettersSparql()
        {
            ValidateSparql(() => controller.ConstituencyAToZ());
        }

        //[TestMethod()]
        //public void ConstituencyCurrentByLettersSparql()
        //{
        //    ValidateSparql(() => controller.ConstituencyCurrentByLetters(string.Empty));
        //}

        [TestMethod()]
        public void ConstituencyCurrentAToZLettersSparql()
        {
            ValidateSparql(() => controller.ConstituencyCurrentAToZ());
        }

        [TestMethod()]
        public void ConstituencyIndexSparql()
        {
            ValidateSparql(() => controller.ConstituencyIndex());
        }

        [TestMethod()]
        public void ConstituencyMembersSparql()
        {
            ValidateSparql(() => controller.ConstituencyMembers(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyCurrentMembersSparql()
        {
            ValidateSparql(() => controller.ConstituencyCurrentMember(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyContactPointSparql()
        {
            ValidateSparql(() => controller.ConstituencyContactPoint(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyLookupByPostcodeSparql()
        {
            ValidateSparql(() => controller.ConstituencyLookupByPostcode(string.Empty));
        }
    }
}