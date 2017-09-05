namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("Party")]
    [TestCategory("Sparql")]
    public class PartySparql : SparqlValidator
    {
        private FixedQueryController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new FixedQueryController();
        }

        [TestMethod()]
        public void PartyIndexSparql()
        {
            ValidateSparql(() => controller.party_index());
        }

        [TestMethod()]
        public void PartyByIdSparql()
        {
            ValidateSparql(() => controller.party_by_id(string.Empty));
        }

        [TestMethod()]
        public void PartyByInitialSparql()
        {
            ValidateSparql(() => controller.party_by_initial(string.Empty));
        }

        [TestMethod()]
        public void PartyCurrentSparql()
        {
            ValidateSparql(() => controller.party_current());
        }

        [TestMethod()]
        public void PartyAToZSparql()
        {
            ValidateSparql(() => controller.party_a_to_z());
        }

        [TestMethod()]
        public void PartyCurrentAToZSparql()
        {
            ValidateSparql(() => controller.party_current_a_to_z());
        }

        [TestMethod()]
        public void PartyLookupSparql()
        {
            ValidateSparql(() => controller.party_lookup(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void PartyBySubstringSparql()
        {
            ValidateSparql(() => controller.party_by_substring(string.Empty));
        }

        [TestMethod()]
        public void PartyMembersSparql()
        {
            ValidateSparql(() => controller.party_members(string.Empty));
        }

        [TestMethod()]
        public void PartyCurrentMembersSparql()
        {
            ValidateSparql(() => controller.party_current_members(string.Empty));
        }

        [TestMethod()]
        public void PartyMembersByInitialSparql()
        {
            ValidateSparql(() => controller.party_members_by_initial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void PartyMembersAToZSparql()
        {
            ValidateSparql(() => controller.party_members_a_to_z(string.Empty));
        }

        [TestMethod()]
        public void PartyCurrentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.party_current_members_by_initial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void PartyCurrentMembersAToZSparql()
        {
            ValidateSparql(() => controller.party_current_members_a_to_z(string.Empty));
        }
    }
}