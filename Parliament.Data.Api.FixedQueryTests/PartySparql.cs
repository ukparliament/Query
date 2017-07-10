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
        private XController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new XController();
        }

        [TestMethod()]
        public void PartyIndexSparql()
        {
            ValidateSparql(() => controller.PartyIndex());
        }

        [TestMethod()]
        public void PartyByIdSparql()
        {
            ValidateSparql(() => controller.PartyById(string.Empty));
        }

        [TestMethod()]
        public void PartyByInitialSparql()
        {
            ValidateSparql(() => controller.PartyByInitial(string.Empty));
        }

        [TestMethod()]
        public void PartyCurrentSparql()
        {
            ValidateSparql(() => controller.PartyCurrent());
        }

        [TestMethod()]
        public void PartyAToZLettersSparql()
        {
            ValidateSparql(() => controller.PartyAToZ());
        }

        [TestMethod()]
        public void PartyCurrentAToZPartiesSparql()
        {
            ValidateSparql(() => controller.PartyCurrentAToZ());
        }

        [TestMethod()]
        public void PartyLookupSparql()
        {
            ValidateSparql(() => controller.PartyLookup(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void PartyByLettersSparql()
        {
            ValidateSparql(() => controller.PartyByLetters(string.Empty));
        }

        [TestMethod()]
        public void PartyMembersSparql()
        {
            ValidateSparql(() => controller.PartyMembers(string.Empty));
        }

        [TestMethod()]
        public void PartyCurrentMembersSparql()
        {
            ValidateSparql(() => controller.PartyCurrentMembers(string.Empty));
        }

        [TestMethod()]
        public void PartyMembersByInitialSparql()
        {
            ValidateSparql(() => controller.PartyMembersByInitial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void PartyMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.PartyMembersAToZ(string.Empty));
        }

        [TestMethod()]
        public void PartyCurrentMembersByInitialSparql()
        {
            ValidateSparql(() => controller.PartyCurrentMembersByInitial(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void PartyCurrentMembersAToZLettersSparql()
        {
            ValidateSparql(() => controller.PartyCurrentMembersAToZ(string.Empty));
        }
    }
}