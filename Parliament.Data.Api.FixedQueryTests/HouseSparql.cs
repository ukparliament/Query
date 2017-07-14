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
        private HouseController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new HouseController();
        }

        [TestMethod()]
        public void HouseByIdSparql()
        {
            ValidateSparql(() => controller.ById(string.Empty));
        }

        [TestMethod()]
        public void HouseLookupSparql()
        {
            ValidateSparql(() => controller.Lookup(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseByLettersSparql()
        {
            ValidateSparql(() => controller.ByLetters(string.Empty));
        }

        [TestMethod()]
        public void HouseIndexSparql()
        {
            ValidateSparql(() => controller.Index());
        }

        [TestMethod()]
        public void HouseMembersSparql()
        {
            ValidateSparql(() => controller.Members(string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentMembersSparql()
        {
            ValidateSparql(() => controller.CurrentMembers(string.Empty));
        }

        [TestMethod()]
        public void HousePartiesSparql()
        {
            ValidateSparql(() => controller.Parties(string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentPartiesSparql()
        {
            ValidateSparql(() => controller.CurrentParties(string.Empty));
        }

        [TestMethod()]
        public void HousePartyByIdSparql()
        {
            ValidateSparql(() => controller.PartyById(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseMembersByInitialSparql()
        {
            ValidateSparql(() => controller.MembersByInitial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.MembersAToZLetters(string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.CurrentMembersByInitial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HouseCurrentMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.CurrentMembersAToZLetters(string.Empty));
        }

        [TestMethod()]
        public void HousePartyMembersSparql()
        {
            ValidateSparql(() => controller.PartyMembers(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyMembersByInitialSparql()
        {
            ValidateSparql(() => controller.PartyMembers(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.PartyMembersAToZLetters(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyCurrentMembersSparql()
        {
            ValidateSparql(() => controller.PartyCurrentMembers(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyCurrentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.PartyCurrentMembersByInitial(string.Empty, string.Empty, string.Empty));
        }

        [TestMethod()]
        public void HousePartyCurrentMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.PartyCurrentMembersAToZLetters(string.Empty, string.Empty));
        }
    }
}