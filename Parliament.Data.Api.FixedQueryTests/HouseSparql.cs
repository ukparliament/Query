﻿namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("House")]
    [TestCategory("Sparql")]
    public class HouseSparql : SparqlValidator
    {
        private FixedQueryController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new FixedQueryController();
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
        public void HouseBySubstringSparql()
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
        public void HouseMembersAToZSparql()
        {
            ValidateSparql(() => controller.house_members_a_to_z(string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.house_current_members_by_initial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousecurrentMembersAToZSparql()
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
        public void HousePartyMembersAToZSparql()
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
        public void HousePartyCurrentMembersAToZSparql()
        {
            ValidateSparql(() => controller.house_party_current_members_a_to_z(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseCommitteesIndex()
        {
            ValidateSparql(() => controller.house_committees_index(string.Empty));
        }

        [TestMethod()]
        public void HouseCommitteesAtoZ()
        {
            ValidateSparql(() => controller.house_committees_a_to_z(string.Empty));
        }

        [TestMethod()]
        public void HouseCommitteesByInitial()
        {
            ValidateSparql(() => controller.house_committees_by_initial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentCommittees()
        {
            ValidateSparql(() => controller.house_current_committees(string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentCommitteesAtoZ()
        {
            ValidateSparql(() => controller.house_current_committees_a_to_z(string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentCommitteesByInitial()
        {
            ValidateSparql(() => controller.house_current_committees_by_initial(string.Empty, string.Empty));
        }
    }
}
