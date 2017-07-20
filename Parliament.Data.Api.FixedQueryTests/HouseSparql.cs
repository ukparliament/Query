namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("House")]
    [TestCategory("Sparql")]
    public class HouseSparql : SparqlValidator
    {
        private XController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new XController();
        }

        [TestMethod()]
        public void HouseByIdSparql()
        {
            ValidateSparql(() => controller.house_by_id(string.Empty));
        }

        [TestMethod()]
        public void HouseLookupSparql()
        {
            ValidateSparql(() => controller.house_lookup(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseByLettersSparql()
        {
            ValidateSparql(() => controller.house_by_substring(string.Empty));
        }

        [TestMethod()]
        public void HouseIndexSparql()
        {
            ValidateSparql(() => controller.house_index());
        }

        [TestMethod()]
        public void HouseMembersSparql()
        {
            ValidateSparql(() => controller.house_members(string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentMembersSparql()
        {
            ValidateSparql(() => controller.house_current_members(string.Empty));
        }

        [TestMethod()]
        public void HousePartiesSparql()
        {
            ValidateSparql(() => controller.house_parties(string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentPartiesSparql()
        {
            ValidateSparql(() => controller.house_current_parties(string.Empty));
        }

        [TestMethod()]
        public void HousePartyByIdSparql()
        {
            ValidateSparql(() => controller.house_party_by_id(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseMembersByInitialSparql()
        {
            ValidateSparql(() => controller.house_members_by_initial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.house_members_a_to_z(string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.house_current_members_by_initial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousecurrentMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.house_current_members_a_to_z(string.Empty));
        }
        [TestMethod()]
        public void HousePartyMembersSparql()
        {
            ValidateSparql(() => controller.house_party_members(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyMembersByInitialSparql()
        {
            ValidateSparql(() => controller.house_party_members_by_initial(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.house_party_members_a_to_z(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyCurrentMembersSparql()
        {
            ValidateSparql(() => controller.house_party_current_members(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyCurrentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.house_party_current_members_by_initial(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyCurrentMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.house_party_current_members_a_to_z(string.Empty, string.Empty));
        }
    }
}