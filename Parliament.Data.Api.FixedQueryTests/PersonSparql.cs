namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("Person")]
    [TestCategory("Sparql")]
    public class PersonSparql : SparqlValidator
    {
        private PersonController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new PersonController();
        }

        [TestMethod()]
        public void PersonIndexSparql()
        {
            ValidateSparql(() => controller.Index());
        }

        [TestMethod()]
        public void PersonByIdSparql()
        {
            ValidateSparql(() => controller.ById(null));
        }

        [TestMethod()]
        public void PersonByInitialSparql()
        {
            ValidateSparql(() => controller.ByInitial(null));
        }

        [TestMethod()]
        public void PersonLookupSparql()
        {
            ValidateSparql(() => controller.Lookup(null, null));
        }

        [TestMethod()]
        public void PersonMemberIndexSparql()
        {
            ValidateSparql(() => controller.MemberIndex());
        }

        [TestMethod()]
        public void PersonByLettersSparql()
        {
            ValidateSparql(() => controller.ByLetters(null));
        }

        [TestMethod()]
        public void PersonAToZLettersSparql()
        {
            ValidateSparql(() => controller.AToZLetters());
        }

        [TestMethod()]
        public void PersonConstituenciesSparql()
        {
            ValidateSparql(() => controller.Constituencies(null));
        }

        [TestMethod()]
        public void PersonCurrentConstituencySparql()
        {
            ValidateSparql(() => controller.CurrentConstituency(null));
        }

        [TestMethod()]
        public void PersonPartiesSparql()
        {
            ValidateSparql(() => controller.Parties(null));
        }

        [TestMethod()]
        public void PersonCurrentPartySparql()
        {
            ValidateSparql(() => controller.CurrentParty(null));
        }

        [TestMethod()]
        public void PersonContactPointsSparql()
        {
            ValidateSparql(() => controller.ContactPoints(null));
        }

        [TestMethod()]
        public void PersonHousesSparql()
        {
            ValidateSparql(() => controller.Houses(null));
        }

        [TestMethod()]
        public void PersonCurrentHouseSparql()
        {
            ValidateSparql(() => controller.CurrentHouse(null));
        }
    }
}