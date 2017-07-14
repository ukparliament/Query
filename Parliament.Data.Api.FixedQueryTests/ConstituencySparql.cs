namespace Parliament.Data.Api.FixedQuery.Controllers.Test

    using Microsoft.VisualStudio.TestTools.UnitTesting
    using Parliament.Data.Api.FixedQuery.Controllers
    using Parliament.Data.Api.FixedQueryTests

    [TestClass()
    [TestCategory("Constituency")
    [TestCategory("Sparql")
    public class ConstituencySparql : SparqlValidato
    
        private ConstituencyController controller

        [TestInitialize
        public void Initialize(
        
            controller = new ConstituencyController()
        

        [TestMethod()
        public void ConstituencyByIdSparql(
        
            ValidateSparql(() => controller.ById(string.Empty))
        

        [TestMethod()
        public void ConstituencyMapSparql(
        
            ValidateSparql(() => controller.Map(string.Empty))
        

        [TestMethod()
        public void ConstituencyByInitialSparql(
        
            ValidateSparql(() => controller.ByInitial(string.Empty))
        

        [TestMethod()
        public void ConstituencyCurrentSparql(
        
            ValidateSparql(() => controller.Current())
        

        [TestMethod()
        public void ConstituencyLookupSparql(
        
            ValidateSparql(() => controller.Lookup(string.Empty, string.Empty))
        

        [TestMethod()
        public void ConstituencyByLettersSparql(
        
            ValidateSparql(() => controller.ByLetters(string.Empty))
        

        [TestMethod()
        public void ConstituencyAToZLettersSparql(
        
            ValidateSparql(() => controller.AToZLetters())
        

        [TestMethod()
        public void ConstituencyCurrentByLettersSparql(
        
            ValidateSparql(() => controller.CurrentByLetters(string.Empty))
        

        [TestMethod()
        public void ConstituencyCurrentAToZLettersSparql(
        
            ValidateSparql(() => controller.CurrentAToZLetters())
        

        [TestMethod()
        public void ConstituencyIndexSparql(
        
            ValidateSparql(() => controller.Index())
        

        [TestMethod()
        public void ConstituencyMembersSparql(
        
            ValidateSparql(() => controller.Members(string.Empty))
        

        [TestMethod()
        public void ConstituencyCurrentMembersSparql(
        
            ValidateSparql(() => controller.CurrentMembers(string.Empty))
        

        [TestMethod()
        public void ConstituencyContactPointSparql(
        
            ValidateSparql(() => controller.ContactPoint(string.Empty))
        

        [TestMethod()
        public void ConstituencyLookupByPostcodeSparql(
        
            ValidateSparql(() => controller.LookupByPostcode(string.Empty))
        
    
}
