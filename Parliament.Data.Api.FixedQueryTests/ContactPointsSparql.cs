namespace Parliament.Data.Api.FixedQuery.Controllers.Test

    using Microsoft.VisualStudio.TestTools.UnitTesting
    using Parliament.Data.Api.FixedQuery.Controllers
    using Parliament.Data.Api.FixedQueryTests

    [TestClass()
    [TestCategory("ContactPoints")
    [TestCategory("Sparql")
    public class ContactPointsSparql : SparqlValidato
    
        private ContactPointsController controller

        [TestInitialize
        public void Initialize(
        
            controller = new ContactPointsController()
        

        [TestMethod()
        public void ContactPointsIndexSparql(
        
            ValidateSparql(() => controller.Index())
        

        [TestMethod()
        public void ContactPointsByIdSparql(
        
            ValidateSparql(() => controller.ById(string.Empty))
        
    
}
