namespace Parliament.Data.Api.FixedQuery.Controllers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using Parliament.Data.Api.FixedQueryTests;

    [TestClass()]
    [TestCategory("Media")]
    [TestCategory("Sparql")]
    public class MediaSparql : SparqlValidator
    {
        private MediaController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new MediaController();
        }

        [TestMethod()]
        public void ImageByIdSparql()
        {
            ValidateSparql(() => controller.ById(string.Empty));
        }
    }
}