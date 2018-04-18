namespace Parliament.Data.Api.FixedQuery.Tests
{
    using Microsoft.OpenApi.Models;
    using Microsoft.OpenApi.Readers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    [TestClass]
    [TestCategory("Endpoint")]
    public class EndpointTests
    {
        private static IEnumerable<object[]> Endpoints
        {
            get
            {
                return Resources.OpenApiDefinition.Paths.Select(p => new object[] { p.Key });
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
        public void EndpointsHaveImplementation(string endpointName)
        {
            string key = endpointName
                .Remove(endpointName.Length - 5, 5)
                .Remove(0, 1);
            OpenApiPathItem endpoint = Resources.GetApiPathItem(key);
            EndpointType endpointType = Resources.GetEndpointType(endpoint);

            if (endpointType == EndpointType.HardCoded)
            {
                var method = typeof(HardCoded).GetMethod(key, BindingFlags.Public | BindingFlags.Static);
                Assert.IsNotNull(method, "Method {0} missing", endpointName);

                var parameters = method.GetParameters();
                Assert.AreEqual(1, parameters.Length, "Hard-coded endpoint methods must have a single parameter.");
                Assert.AreEqual(typeof(Dictionary<string, string>), parameters.Single().ParameterType, "Hard-coded endpoint method parameter must be a Dictionary<string, string>.");
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
            OpenApiDocument document = Resources.OpenApiDefinition;
            OpenApiDiagnostic diagnostic = Resources.ApiDiagnostic;

            Assert.IsFalse(diagnostic.Errors.Any(), string.Join(",", diagnostic.Errors));
        }

        [TestMethod]
        [DynamicData(nameof(SparqlFileNames))]
        public void ImplementationHasEndpoint(string sparqlName)
        {
            OpenApiPathItem pathItem = Resources.GetApiPathItem(sparqlName);
            Assert.IsNotNull(pathItem);
        }
    }
}