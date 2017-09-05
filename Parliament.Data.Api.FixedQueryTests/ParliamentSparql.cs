namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("Parliament")]
    [TestCategory("Sparql")]
    public class ParliamentSparql : SparqlValidator
    {
        private FixedQueryController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new FixedQueryController();
        }

        [TestMethod()]
        public void ParliamentIndexSparql()
        {
            ValidateSparql(() => controller.parliament_index());
        }

        [TestMethod()]
        public void ParliamentCurrentSparql()
        {
            ValidateSparql(() => controller.parliament_current());
        }

        [TestMethod()]
        public void ParliamentPreviousSparql()
        {
            ValidateSparql(() => controller.parliament_previous());
        }

        [TestMethod()]
        public void PreviousParliamentByIDSparql()
        {
            ValidateSparql(() => controller.previous_parliament_by_id(string.Empty));
        }

        [TestMethod()]
        public void ParliamentNextSparql()
        {
            ValidateSparql(() => controller.parliament_next());
        }

        [TestMethod()]
        public void ParliamentLookupSparql()
        {
            ValidateSparql(() => controller.parliament_lookup(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentByIdSparql()
        {
            ValidateSparql(() => controller.parliament_by_id(string.Empty));
        }

        [TestMethod()]
        public void NextParliamentByIdSparql()
        {
            ValidateSparql(() => controller.next_parliament_by_id(string.Empty));
        }

        [TestMethod()]
        public void ParliamentMembersSparql()
        {
            ValidateSparql(() => controller.parliament_members(string.Empty));
        }

        [TestMethod()]
        public void ParliamentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.parliament_members_by_initial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentMembersAToZSparql()
        {
            ValidateSparql(() => controller.parliament_members_a_to_z(string.Empty));
        }

        [TestMethod()]
        public void ParliamentHousesSparql()
        {
            ValidateSparql(() => controller.parliament_houses(string.Empty));
        }

        [TestMethod()]
        public void ParliamentHouseSparql()
        {
            ValidateSparql(() => controller.parliament_house(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHouseMembersSparql()
        {
            ValidateSparql(() => controller.parliament_house_members(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHouseMembersAToZSparql()
        {
            ValidateSparql(() => controller.parliament_house_members_a_to_z(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHouseMembersByInitialSparql()
        {
            ValidateSparql(() => controller.parliament_house_members_by_initial(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentPartiesSparql()
        {
            ValidateSparql(() => controller.parliament_parties(string.Empty));
        }

        [TestMethod()]
        public void ParliamentPartySparql()
        {
            ValidateSparql(() => controller.parliament_party(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentPartyMembersSparql()
        {
            ValidateSparql(() => controller.parliament_party_members(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentPartyMembersAToZSparql()
        {
            ValidateSparql(() => controller.parliament_party_members_a_to_z(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentPartyMembersByInitialSparql()
        {
            ValidateSparql(() => controller.parliament_party_members_by_initial(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHousePartiesSparql()
        {
            ValidateSparql(() => controller.parliament_house_parties(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHousePartySparql()
        {
            ValidateSparql(() => controller.parliament_house_party(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHousePartyMembersSparql()
        {
            ValidateSparql(() => controller.parliament_house_party_members(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHousePartyMembersAToZSparql()
        {
            ValidateSparql(() => controller.parliament_house_party_members_a_to_z(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHousePartyMembersByInitialSparql()
        {
            ValidateSparql(() => controller.parliament_house_party_members_by_initial(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentConstituenciesSparql()
        {
            ValidateSparql(() => controller.parliament_constituencies(string.Empty));
        }

        [TestMethod()]
        public void ParliamentConstituenciesAToZSparql()
        {
            ValidateSparql(() => controller.parliament_constituencies_a_to_z(string.Empty));
        }

        [TestMethod()]
        public void ParliamentConstituenciesByInitialSparql()
        {
            ValidateSparql(() => controller.parliament_constituencies_by_initial(string.Empty, string.Empty));
        }
    }
}