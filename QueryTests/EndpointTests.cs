namespace QueryTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Query;

    [TestClass]
    [TestCategory("Endpoint")]
    public class EndpointTests
    {
        private static IEnumerable<object[]> Endpoints
        {
            get
            {
                return Resources.OpenApiDocument.Paths.Select(p => new object[] { p.Key });
            }
        }

        private static IEnumerable<object[]> SparqlFileNames
        {
            get
            {
                return Resources.SparqlFileNames.Select(fileName => new[] { fileName });
            }
        }

        [TestMethod]
        [DynamicData(nameof(Endpoints))]
        public void EndpointHasImplementation(string endpointName)
        {
            var key = endpointName
                .Remove(endpointName.Length - 5, 5)
                .Remove(0, 1);

            var endpoint = Resources.OpenApiDocument.Paths[key];
            var endpointType = Resources.GetXType<EndpointType>(endpoint);

            if (endpointType == EndpointType.HardCoded)
            {
                var method = typeof(HardCoded).GetMethod(key, BindingFlags.Public | BindingFlags.Static);
                Assert.IsNotNull(method, "Public static method {0} missing on class HardCoded", endpointName);

                var parameters = method.GetParameters();
                Assert.AreEqual(1, parameters.Length, "Hard-coded endpoint methods must have a single parameter.");
                Assert.IsInstanceOfType(parameters.Single(), typeof(Dictionary<string, string>), "Hard-coded endpoint method parameter must be a Dictionary<string, string>.");
            }
            else
            {
                var queryString = Resources.GetSparql(key);

                foreach (var parameter in Resources.GetSparqlParameters(endpoint))
                {
                    Assert.IsTrue(queryString.Contains($"@{parameter.Name}"), "Parameter @{0} missing in query {1}", parameter.Name, endpointName);
                }

                Assert.IsNotNull(queryString, "SPARQL file missing");
            }
        }

        [TestMethod]
        public void EndpointsJsonIsValid()
        {
            var document = Resources.OpenApiDocument;
        }

        [TestMethod]
        [DynamicData(nameof(SparqlFileNames))]
        public void ImplementationHasEndpoint(string sparqlName)
        {
            Assert.IsNotNull(Resources.OpenApiDocument.Paths[sparqlName]);
        }
    }
}