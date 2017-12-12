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
    public class EndpointTests
    {
        private static IEnumerable<object[]> Endpoints
        {
            get
            {
                return Resources.DB.Endpoints.Select(endpoint => new object[] { endpoint.Key });
            }
        }

        [TestMethod]
        [DynamicData("Endpoints")]
        public void EndpointsHaveImplementation(string endpointName)
        {
            var endpoint = Resources.DB.Endpoints[endpointName];

            if (endpoint.Type == EndpointType.HardCoded)
            {
                var method = typeof(HardCoded).GetMethod(endpointName, BindingFlags.Public | BindingFlags.Static);

                Assert.IsNotNull(method, "Method {0} missing", endpointName);

                var methodParameters = method.GetParameters();

                foreach (var parameterName in endpoint.Parameters.Keys)
                {
                    var methodParameter = methodParameters.SingleOrDefault(parameterInfo => parameterInfo.Name == parameterName);

                    Assert.IsNotNull(methodParameter, "Parameter {0} missing on method {1}", parameterName, method.Name);
                }
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
    }
}