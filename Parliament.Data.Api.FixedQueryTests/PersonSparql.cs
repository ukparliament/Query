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
        private FixedQueryController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new FixedQueryController();
        }

        [TestMethod()]
        public void PersonIndexSparql()
        {
            ValidateSparql(() => controller.person_index());
        }

        [TestMethod()]
        public void PersonByIdSparql()
        {
            ValidateSparql(() => controller.person_by_id(string.Empty));
        }

        [TestMethod()]
        public void PersonByInitialSparql()
        {
            ValidateSparql(() => controller.person_by_initial(string.Empty));
        }

        [TestMethod()]
        public void PersonLookupSparql()
        {
            ValidateSparql(() => controller.person_lookup(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void PersonBySubstringSparql()
        {
            ValidateSparql(() => controller.person_by_substring(string.Empty));
        }

        [TestMethod()]
        public void PersonAToZSparql()
        {
            ValidateSparql(() => controller.person_a_to_z());
        }

        [TestMethod()]
        public void PersonConstituenciesSparql()
        {
            ValidateSparql(() => controller.person_constituencies(string.Empty));
        }

        [TestMethod()]
        public void PersonCurrentConstituencySparql()
        {
            ValidateSparql(() => controller.person_current_constituency(string.Empty));
        }

        [TestMethod()]
        public void PersonPartiesSparql()
        {
            ValidateSparql(() => controller.person_parties(string.Empty));
        }

        [TestMethod()]
        public void PersonCurrentPartySparql()
        {
            ValidateSparql(() => controller.person_current_party(string.Empty));
        }

        [TestMethod()]
        public void PersonContactPointsSparql()
        {
            ValidateSparql(() => controller.person_contact_points(string.Empty));
        }

        [TestMethod()]
        public void PersonHousesSparql()
        {
            ValidateSparql(() => controller.person_houses(string.Empty));
        }

        [TestMethod()]
        public void PersonCurrentHouseSparql()
        {
            ValidateSparql(() => controller.person_current_house(string.Empty));
        }

        [TestMethod()]
        public void PersonMPsSparql()
        {
            ValidateSparql(() => controller.person_mps());
        }
    }
}
