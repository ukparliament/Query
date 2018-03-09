namespace Parliament.Data.Api.FixedQuery.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Schema;
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
                return Resources.DB.Endpoints.Select(endpoint => new object[] { endpoint.Key });
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
            var endpoint = Resources.DB.Endpoints[endpointName];

            if (endpoint.Type == EndpointType.HardCoded)
            {
                var method = typeof(HardCoded).GetMethod(endpointName, BindingFlags.Public | BindingFlags.Static);
                Assert.IsNotNull(method, "Method {0} missing", endpointName);

                var parameters = method.GetParameters();
                Assert.AreEqual(1, parameters.Length,"Hard-coded endpoint methods must have a single parameter.");
                Assert.AreEqual(typeof(Dictionary<string, string>), parameters.Single().ParameterType, "Hard-coded endpoint method parameter must be a Dictionary<string, string>.");
            }
            else
            {
                var queryString = Resources.GetSparql(endpointName);

                foreach (var parameterName in endpoint.Parameters.Keys)
                {
                    Assert.IsTrue(queryString.Contains($"@{parameterName}"), "Parameter @{0} missing in query {1}", parameterName, endpointName);
                }

                Assert.IsNotNull(queryString, "SPARQL file missing");
            }
        }

        [TestMethod]
        public void EndpointsJsonIsValid()
        {
            var endpointsJson = JObject.Parse(Resources.EndpointsJson);
            var endpointsSchema = JSchema.Parse(Resources.EndpointsSchema);

            var result = endpointsJson.IsValid(endpointsSchema, out IList<string> errors);

            Assert.IsTrue(result, string.Join(",", errors));
        }

        [TestMethod]
        [DynamicData(nameof(SparqlFileNames))]
        public void ImplementationHasEndpoint(string sparqlName)
        {
            CollectionAssert.Contains(Resources.DB.Endpoints.Keys, sparqlName);
        }
    }
}