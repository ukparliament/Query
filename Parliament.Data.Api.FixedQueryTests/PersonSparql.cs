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
        private XController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new XController();
        }

        [TestMethod()]
        public void PersonIndexSparql()
        {
            ValidateSparql(() => controller.PersonIndex());
        }

        [TestMethod()]
        public void PersonByIdSparql()
        {
            ValidateSparql(() => controller.PersonById(string.Empty));
        }

        [TestMethod()]
        public void PersonByInitialSparql()
        {
            ValidateSparql(() => controller.PersonByInitial(string.Empty));
        }

        [TestMethod()]
        public void PersonLookupSparql()
        {
            ValidateSparql(() => controller.PersonLookup(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void PersonMemberIndexSparql()
        {
            ValidateSparql(() => controller.PersonIndex());
        }

        [TestMethod()]
        public void PersonByLettersSparql()
        {
            ValidateSparql(() => controller.PersonByLetters(string.Empty));
        }

        [TestMethod()]
        public void PersonAToZLettersSparql()
        {
            ValidateSparql(() => controller.PersonAToZ());
        }

        [TestMethod()]
        public void PersonConstituenciesSparql()
        {
            ValidateSparql(() => controller.PersonConstituencies(string.Empty));
        }

        [TestMethod()]
        public void PersonCurrentConstituencySparql()
        {
            ValidateSparql(() => controller.PersonCurrentConstituency(string.Empty));
        }

        [TestMethod()]
        public void PersonPartiesSparql()
        {
            ValidateSparql(() => controller.PersonParties(string.Empty));
        }

        [TestMethod()]
        public void PersonCurrentPartySparql()
        {
            ValidateSparql(() => controller.PersonCurrentParty(string.Empty));
        }

        [TestMethod()]
        public void PersonContactPointsSparql()
        {
            ValidateSparql(() => controller.PersonContactPoints(string.Empty));
        }

        [TestMethod()]
        public void PersonHousesSparql()
        {
            ValidateSparql(() => controller.PersonHouses(string.Empty));
        }

        [TestMethod()]
        public void PersonCurrentHouseSparql()
        {
            ValidateSparql(() => controller.PersonCurrentHouse(string.Empty));
        }

        [TestMethod()]
        public void PersonMPsSparql()
        {
            ValidateSparql(() => controller.PersonMPs());
        }
    }
}