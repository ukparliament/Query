namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    public class ConstituencyControllerSparql : SparqlValidator
    {
        private ConstituencyController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new ConstituencyController();
        }

        [TestMethod()]
        public void ById()
        {
            ValidateSparql(() => controller.ById(null));
        }

        [TestMethod()]
        public void ByInitial()
        {
            ValidateSparql(() => controller.ByInitial(null));
        }

        [TestMethod()]
        public void Current()
        {
            ValidateSparql(() => controller.Current());
        }

        [TestMethod()]
        public void Lookup()
        {
            ValidateSparql(() => controller.Lookup(null, null));
        }

        [TestMethod()]
        public void ByLetters()
        {
            ValidateSparql(() => controller.ByLetters(null));
        }

        [TestMethod()]
        public void AToZLetters()
        {
            ValidateSparql(() => controller.AToZLetters());
        }

        [TestMethod()]
        public void CurrentByLetters()
        {
            ValidateSparql(() => controller.CurrentByLetters(null));
        }

        [TestMethod()]
        public void CurrentAToZLetters()
        {
            ValidateSparql(() => controller.CurrentAToZLetters());
        }

        [TestMethod()]
        public void Index()
        {
            ValidateSparql(() => controller.Index());
        }

        [TestMethod()]
        public void Members()
        {
            ValidateSparql(() => controller.Members(null));
        }

        [TestMethod()]
        public void CurrentMembers()
        {
            ValidateSparql(() => controller.CurrentMembers(null));
        }

        [TestMethod()]
        public void ContactPoint()
        {
            ValidateSparql(() => controller.ContactPoint(null));
        }

        [TestMethod()]
        public void LookupByPostcode()
        {
            ValidateSparql(() => controller.LookupByPostcode(null));
        }
    }
}