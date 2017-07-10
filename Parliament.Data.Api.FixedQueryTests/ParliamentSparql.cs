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
        private XController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new XController();
        }

        [TestMethod()]
        public void ParliamentIndexSparql()
        {
            ValidateSparql(() => controller.ParliamentIndex());
        }

        [TestMethod()]
        public void ParliamentCurrentSparql()
        {
            ValidateSparql(() => controller.ParliamentCurrent());
        }

        [TestMethod()]
        public void ParliamentPreviousSparql()
        {
            ValidateSparql(() => controller.ParliamentPrevious());
        }

        [TestMethod()]
        public void ParliamentNextSparql()
        {
            ValidateSparql(() => controller.ParliamentNext());
        }

        [TestMethod()]
        public void ParliamentLookupSparql()
        {
            ValidateSparql(() => controller.ParliamentLookup(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentByIdSparql()
        {
            ValidateSparql(() => controller.ParliamentById(string.Empty));
        }

        [TestMethod()]
        public void ParliamentNextByIdSparql()
        {
            ValidateSparql(() => controller.ParliamentNext());
        }

        [TestMethod()]
        public void ParliamentMembersSparql()
        {
            ValidateSparql(() => controller.ParliamentMembers(string.Empty));
        }

        [TestMethod()]
        public void ParliamentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.ParliamentMembersByInitial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentMembersAToZSparql()
        {
            ValidateSparql(() => controller.ParliamentMembersAToZLetters(string.Empty));
        }

        [TestMethod()]
        public void ParliamentHousesSparql()
        {
            ValidateSparql(() => controller.ParliamentHouses(string.Empty));
        }

        [TestMethod()]
        public void ParliamentHouseSparql()
        {
            ValidateSparql(() => controller.ParliamentHouse(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHouseMembersSparql()
        {
            ValidateSparql(() => controller.ParliamentHouseMembers(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHouseMembersAToZSparql()
        {
            ValidateSparql(() => controller.ParliamentHouseMembersAToZLetters(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHouseMembersByInitialSparql()
        {
            ValidateSparql(() => controller.ParliamentHouseMembersByInitial(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentPartiesSparql()
        {
            ValidateSparql(() => controller.ParliamentParties(string.Empty));
        }

        [TestMethod()]
        public void ParliamentPartySparql()
        {
            ValidateSparql(() => controller.ParliamentParty(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentPartyMembersSparql()
        {
            ValidateSparql(() => controller.ParliamentPartyMembers(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentPartyMembersAToZSparql()
        {
            ValidateSparql(() => controller.ParliamentPartyMembersAToZLetters(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentPartyMembersByInitialSparql()
        {
            ValidateSparql(() => controller.ParliamentPartyMembersByInitial(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHousePartiesSparql()
        {
            ValidateSparql(() => controller.ParliamentHouseParties(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHousePartySparql()
        {
            ValidateSparql(() => controller.ParliamentHouseParty(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHousePartyMembersSparql()
        {
            ValidateSparql(() => controller.ParliamentHousePartyMembers(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHousePartyMembersAToZSparql()
        {
            ValidateSparql(() => controller.ParliamentHousePartyMembersAToZLetters(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentHousePartyMembersByInitialSparql()
        {
            ValidateSparql(() => controller.ParliamentHousePartyMembersByInitial(string.Empty, string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void ParliamentConstituenciesSparql()
        {
            ValidateSparql(() => controller.ParliamentConstituencies(string.Empty));
        }

        [TestMethod()]
        public void ParliamentConstituenciesAToZSparql()
        {
            ValidateSparql(() => controller.ParliamentConstituenciesAToZLetters(string.Empty));
        }

        [TestMethod()]
        public void ParliamentConstituenciesByInitialSparql()
        {
            ValidateSparql(() => controller.ParliamentConstituenciesByInitial(string.Empty, string.Empty));
        }
    }
}