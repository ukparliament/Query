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
            ValidateSparql(() => controller.ById(string.Empty));
        }

        [TestMethod()]
        public void PersonByInitialSparql()
        {
            ValidateSparql(() => controller.ByInitial(string.Empty));
        }

        [TestMethod()]
        public void PersonLookupSparql()
        {
            ValidateSparql(() => controller.Lookup(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void PersonMemberIndexSparql()
        {
            ValidateSparql(() => controller.Index());
        }

        [TestMethod()]
        public void PersonByLettersSparql()
        {
            ValidateSparql(() => controller.ByLetters(string.Empty));
        }

        [TestMethod()]
        public void PersonAToZLettersSparql()
        {
            ValidateSparql(() => controller.AToZLetters());
        }

        [TestMethod()]
        public void PersonConstituenciesSparql()
        {
            ValidateSparql(() => controller.Constituencies(string.Empty));
        }

        [TestMethod()]
        public void PersonCurrentConstituencySparql()
        {
            ValidateSparql(() => controller.CurrentConstituency(string.Empty));
        }

        [TestMethod()]
        public void PersonPartiesSparql()
        {
            ValidateSparql(() => controller.Parties(string.Empty));
        }

        [TestMethod()]
        public void PersonCurrentPartySparql()
        {
            ValidateSparql(() => controller.CurrentParty(string.Empty));
        }

        [TestMethod()]
        public void PersonContactPointsSparql()
        {
            ValidateSparql(() => controller.ContactPoints(string.Empty));
        }

        [TestMethod()]
        public void PersonHousesSparql()
        {
            ValidateSparql(() => controller.Houses(string.Empty));
        }

        [TestMethod()]
        public void PersonCurrentHouseSparql()
        {
            ValidateSparql(() => controller.CurrentHouse(string.Empty));
        }

        [TestMethod()]
        public void PersonMPsSparql()
        {
            ValidateSparql(() => controller.MPs());
        }
    }
}