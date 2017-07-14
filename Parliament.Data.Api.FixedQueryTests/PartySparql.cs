namespace Parliament.Data.Api.FixedQuery.Controllers.Test

    using Microsoft.VisualStudio.TestTools.UnitTesting
    using Parliament.Data.Api.FixedQuery.Controllers
    using Parliament.Data.Api.FixedQueryTests

    [TestClass()
    [TestCategory("Party")
    [TestCategory("Sparql")
    public class PartySparql : SparqlValidato
    
        private PartyController controller

        [TestInitialize
        public void Initialize(
        
            controller = new PartyController()
        

        [TestMethod()
        public void PartyIndexSparql(
        
            ValidateSparql(() => controller.Index())
        

        [TestMethod()
        public void PartyByIdSparql(
        
            ValidateSparql(() => controller.ById(string.Empty))
        

        [TestMethod()
        public void PartyByInitialSparql(
        
            ValidateSparql(() => controller.ByInitial(string.Empty))
        

        [TestMethod()
        public void PartyCurrentSparql(
        
            ValidateSparql(() => controller.Current())
        

        [TestMethod()
        public void PartyAToZLettersSparql(
        
            ValidateSparql(() => controller.AToZLetters())
        

        [TestMethod()
        public void PartyCurrentAToZPartiesSparql(
        
            ValidateSparql(() => controller.CurrentAToZParties())
        

        [TestMethod()
        public void PartyLookupSparql(
        
            ValidateSparql(() => controller.Lookup(string.Empty, string.Empty))
        

        [TestMethod()
        public void PartyByLettersSparql(
        
            ValidateSparql(() => controller.ByLetters(string.Empty))
        

        [TestMethod()
        public void PartyMembersSparql(
        
            ValidateSparql(() => controller.Members(string.Empty))
        

        [TestMethod()
        public void PartyCurrentMembersSparql(
        
            ValidateSparql(() => controller.CurrentMembers(string.Empty))
        

        [TestMethod()
        public void PartyMembersByInitialSparql(
        
            ValidateSparql(() => controller.MembersByInitial(string.Empty, string.Empty))
        

        [TestMethod()
        public void PartyMembersAToZLettersSparql(
        
            ValidateSparql(() => controller.MembersAToZLetters(string.Empty))
        

        [TestMethod()
        public void PartyCurrentMembersByInitialSparql(
        
            ValidateSparql(() => controller.CurrentMembersByInitial(string.Empty, string.Empty))
        

        [TestMethod()
        public void PartyCurrentMembersAToZLettersSparql(
        
            ValidateSparql(() => controller.CurrentMembersAToZLetters(string.Empty))
        
    
}
