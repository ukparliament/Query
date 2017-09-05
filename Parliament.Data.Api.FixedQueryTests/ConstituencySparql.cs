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
        private FixedQueryController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new FixedQueryController();
        }

        [TestMethod()]
        public void ConstituencyByIdSparql()
        {
            ValidateSparql(() => controller.constituency_by_id(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyCurrentByInitialSparql()
        {
            ValidateSparql(() => controller.constituency_current_by_initial(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyMapSparql()
        {
            ValidateSparql(() => controller.constituency_map(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyByInitialSparql()
        {
            ValidateSparql(() => controller.constituency_by_initial(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyCurrentSparql()
        {
            ValidateSparql(() => controller.constituency_current());
        }

        [TestMethod()]
        public void ConstituencyLookupSparql()
        {
            ValidateSparql(() => controller.constituency_lookup(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ConstituencyBySubstringSparql()
        {
            ValidateSparql(() => controller.constituency_by_substring(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyAToZSparql()
        {
            ValidateSparql(() => controller.constituency_a_to_z());
        }

        [TestMethod()]
        public void ConstituencyCurrentAToZSparql()
        {
            ValidateSparql(() => controller.constituency_current_a_to_z());
        }

        [TestMethod()]
        public void ConstituencyIndexSparql()
        {
            ValidateSparql(() => controller.constituency_index());
        }

        [TestMethod()]
        public void ConstituencyMembersSparql()
        {
            ValidateSparql(() => controller.constituency_members(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyCurrentMemberSparql()
        {
            ValidateSparql(() => controller.constituency_current_member(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyContactPointSparql()
        {
            ValidateSparql(() => controller.constituency_contact_point(string.Empty));
        }

        [TestMethod()]
        public void ConstituencyLookupByPostcodeSparql()
        {
            ValidateSparql(() => controller.constituency_lookup_by_postcode(string.Empty));
        }

        [TestMethod()]
        public void FindYourConstituencySparql()
        {
            ValidateSparql(() => controller.find_your_constituency());
        }
    }
}
