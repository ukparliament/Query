namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
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
            ValidateSparql(() => controller.ById(null));
        }

        [TestMethod()]
        public void HouseLookupSparql()
        {
            ValidateSparql(() => controller.Lookup(null, null));
        }

        [TestMethod()]
        public void HouseByLettersSparql()
        {
            ValidateSparql(() => controller.ByLetters(null));
        }

        [TestMethod()]
        public void HouseIndexSparql()
        {
            ValidateSparql(() => controller.Index());
        }

        [TestMethod()]
        public void HouseMembersSparql()
        {
            ValidateSparql(() => controller.Members(null));
        }

        [TestMethod()]
        public void HouseCurrentMembersSparql()
        {
            ValidateSparql(() => controller.CurrentMembers(null));
        }

        [TestMethod()]
        public void HousePartiesSparql()
        {
            ValidateSparql(() => controller.Parties(null));
        }

        [TestMethod()]
        public void HouseCurrentPartiesSparql()
        {
            ValidateSparql(() => controller.CurrentParties(null));
        }

        [TestMethod()]
        public void HousePartyByIdSparql()
        {
            ValidateSparql(() => controller.PartyById(null, null));
        }

        [TestMethod()]
        public void HouseMembersByInitialSparql()
        {
            ValidateSparql(() => controller.MembersByInitial(null, null));
        }

        [TestMethod()]
        public void HouseMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.MembersAToZLetters(null));
        }

        [TestMethod()]
        public void HouseCurrentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.CurrentMembersByInitial(null, null));
        }

        [TestMethod()]
        public void HouseCurrentMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.CurrentMembersAToZLetters(null));
        }

        [TestMethod()]
        public void HousePartyMembersSparql()
        {
            ValidateSparql(() => controller.PartyMembers(null, null));
        }

        [TestMethod()]
        public void HousePartyMembersByInitialSparql()
        {
            ValidateSparql(() => controller.PartyMembers(null, null, null));
        }

        [TestMethod()]
        public void HousePartyMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.PartyMembersAToZLetters(null, null));
        }

        [TestMethod()]
        public void HousePartyCurrentMembersSparql()
        {
            ValidateSparql(() => controller.PartyCurrentMembers(null, null));
        }

        [TestMethod()]
        public void HousePartyCurrentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.PartyCurrentMembersByInitial(null, null, null));
        }

        [TestMethod()]
        public void HousePartyCurrentMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.PartyCurrentMembersAToZLetters(null, null));
        }
    }
}