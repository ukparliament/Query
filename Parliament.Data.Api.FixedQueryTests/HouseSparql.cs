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
            ValidateSparql(() => controller.HouseById(string.Empty));
        }

        [TestMethod()]
        public void HouseLookupSparql()
        {
            ValidateSparql(() => controller.HouseLookup(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseByLettersSparql()
        {
            ValidateSparql(() => controller.HouseByLetters(string.Empty));
        }

        [TestMethod()]
        public void HouseIndexSparql()
        {
            ValidateSparql(() => controller.HouseIndex());
        }

        [TestMethod()]
        public void HouseMembersSparql()
        {
            ValidateSparql(() => controller.HouseMembers(string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentMembersSparql()
        {
            ValidateSparql(() => controller.HouseCurrentMembers(string.Empty));
        }

        [TestMethod()]
        public void HousePartiesSparql()
        {
            ValidateSparql(() => controller.HouseParties(string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentPartiesSparql()
        {
            ValidateSparql(() => controller.HouseCurrentParties(string.Empty));
        }

        [TestMethod()]
        public void HousePartyByIdSparql()
        {
            ValidateSparql(() => controller.HousePartyById(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseMembersByInitialSparql()
        {
            ValidateSparql(() => controller.HouseMembersByInitial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.HouseMembersAToZ(string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.HouseCurrentMembersByInitial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.HouseCurrentMembersAToZ(string.Empty));
        }

        [TestMethod()]
        public void HousePartyMembersSparql()
        {
            ValidateSparql(() => controller.HousePartyMembers(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyMembersByInitialSparql()
        {
            ValidateSparql(() => controller.HousePartyMembersByInitial(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.HousePartyMembersAToZ(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyCurrentMembersSparql()
        {
            ValidateSparql(() => controller.HousePartyCurrentMembers(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyCurrentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.HousePartyCurrentMembersByInitial(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyCurrentMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.HousePartyCurrentMembersAToZ(string.Empty, string.Empty));
        }
    }
}