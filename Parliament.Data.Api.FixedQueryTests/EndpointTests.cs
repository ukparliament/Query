// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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