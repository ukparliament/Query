namespace Parliament.Data.Api.FixedQuery.Controllers.Test

    using Microsoft.VisualStudio.TestTools.UnitTesting
    using Parliament.Data.Api.FixedQuery.Controllers
    using Parliament.Data.Api.FixedQueryTests

    [TestClass()
    [TestCategory("Parliament")
    [TestCategory("Sparql")
    public class ParliamentSparql : SparqlValidato
    
        private ParliamentController controller

        [TestInitialize
        public void Initialize(
        
            controller = new ParliamentController()
        

        [TestMethod()
        public void ParliamentIndexSparql(
        
            ValidateSparql(() => controller.Index())
        

        [TestMethod()
        public void ParliamentCurrentSparql(
        
            ValidateSparql(() => controller.Current())
        

        [TestMethod()
        public void ParliamentPreviousSparql(
        
            ValidateSparql(() => controller.Previous())
        

        [TestMethod()
        public void ParliamentNextSparql(
        
            ValidateSparql(() => controller.Next())
        

        [TestMethod()
        public void ParliamentLookupSparql(
        
            ValidateSparql(() => controller.Lookup(string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentByIdSparql(
        
            ValidateSparql(() => controller.ById(string.Empty))
        

        [TestMethod()
        public void ParliamentNextByIdSparql(
        
            ValidateSparql(() => controller.Next(string.Empty))
        

        [TestMethod()
        public void ParliamentMembersSparql(
        
            ValidateSparql(() => controller.Members(string.Empty))
        

        [TestMethod()
        public void ParliamentMembersByInitialSparql(
        
            ValidateSparql(() => controller.MembersByInitial(string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentMembersAToZSparql(
        
            ValidateSparql(() => controller.MembersAToZLetters(string.Empty))
        

        [TestMethod()
        public void ParliamentHousesSparql(
        
            ValidateSparql(() => controller.Houses(string.Empty))
        

        [TestMethod()
        public void ParliamentHouseSparql(
        
            ValidateSparql(() => controller.House(string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentHouseMembersSparql(
        
            ValidateSparql(() => controller.HouseMembers(string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentHouseMembersAToZSparql(
        
            ValidateSparql(() => controller.HouseMembersAToZLetters(string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentHouseMembersByInitialSparql(
        
            ValidateSparql(() => controller.HouseMembersByInitial(string.Empty, string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentPartiesSparql(
        
            ValidateSparql(() => controller.Parties(string.Empty))
        

        [TestMethod()
        public void ParliamentPartySparql(
        
            ValidateSparql(() => controller.Party(string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentPartyMembersSparql(
        
            ValidateSparql(() => controller.PartyMembers(string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentPartyMembersAToZSparql(
        
            ValidateSparql(() => controller.PartyMembersAToZLetters(string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentPartyMembersByInitialSparql(
        
            ValidateSparql(() => controller.PartyMembersByInitial(string.Empty, string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentHousePartiesSparql(
        
            ValidateSparql(() => controller.HouseParties(string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentHousePartySparql(
        
            ValidateSparql(() => controller.HouseParty(string.Empty, string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentHousePartyMembersSparql(
        
            ValidateSparql(() => controller.HousePartyMembers(string.Empty, string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentHousePartyMembersAToZSparql(
        
            ValidateSparql(() => controller.HousePartyMembersAToZLetters(string.Empty, string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentHousePartyMembersByInitialSparql(
        
            ValidateSparql(() => controller.HousePartyMembersByInitial(string.Empty, string.Empty, string.Empty, string.Empty))
        

        [TestMethod()
        public void ParliamentConstituenciesSparql(
        
            ValidateSparql(() => controller.Constituencies(string.Empty))
        

        [TestMethod()
        public void ParliamentConstituenciesAToZSparql(
        
            ValidateSparql(() => controller.ConstituenciesAToZLetters(string.Empty))
        

        [TestMethod()
        public void ParliamentConstituenciesByInitialSparql(
        
            ValidateSparql(() => controller.ConstituenciesByInitial(string.Empty, string.Empty))
        
    
}
