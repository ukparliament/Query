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
        private PartyController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new PartyController();
        }

        [TestMethod()]
        public void PartyIndexSparql()
        {
            ValidateSparql(() => controller.Index());
        }

        [TestMethod()]
        public void PartyByIdSparql()
        {
            ValidateSparql(() => controller.ById(null));
        }

        [TestMethod()]
        public void PartyByInitialSparql()
        {
            ValidateSparql(() => controller.ByInitial(null));
        }

        [TestMethod()]
        public void PartyCurrentSparql()
        {
            ValidateSparql(() => controller.Current());
        }

        [TestMethod()]
        public void PartyAToZLettersSparql()
        {
            ValidateSparql(() => controller.AToZLetters());
        }

        [TestMethod()]
        public void PartyCurrentAToZPartiesSparql()
        {
            ValidateSparql(() => controller.CurrentAToZParties());
        }

        [TestMethod()]
        public void PartyLookupSparql()
        {
            ValidateSparql(() => controller.Lookup(null, null));
        }

        [TestMethod()]
        public void PartyByLettersSparql()
        {
            ValidateSparql(() => controller.ByLetters(null));
        }

        [TestMethod()]
        public void PartyMembersSparql()
        {
            ValidateSparql(() => controller.Members(null));
        }

        [TestMethod()]
        public void PartyCurrentMembersSparql()
        {
            ValidateSparql(() => controller.CurrentMembers(null));
        }

        [TestMethod()]
        public void PartyMembersByInitialSparql()
        {
            ValidateSparql(() => controller.MembersByInitial(null, null));
        }

        [TestMethod()]
        public void PartyMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.MembersAToZLetters(null));
        }

        [TestMethod()]
        public void PartyCurrentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.CurrentMembersByInitial(null, null));
        }

        [TestMethod()]
        public void PartyCurrentMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.CurrentMembersAToZLetters(null));
        }
    }
}