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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Parliament.Data.Api.FixedQuery.Controllers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using VDS.RDF.Parsing.Validation;
    using VDS.RDF.Query;

    [TestClass]
    [TestCategory("Sparql")]
    public class SparqlTests
    {
        private static IEnumerable<object[]> Endpoints
        {
            get
            {
                return Resources.OpenApiDefinition.Paths.Select(endpoint => new object[] { endpoint.Key });
            }
        }

        [TestMethod]
        [DynamicData("Endpoints")]
        public void ValidateSparql(string endpointName)
        {
            string key = endpointName
                .Remove(endpointName.Length - 5, 5)
                .Remove(0, 1);
            OpenApiPathItem endpoint = Resources.GetApiPathItem(key);
            EndpointType endpointType = Resources.GetEndpointType(endpoint);

            if (endpointType == EndpointType.HardCoded)
            {
                return;
            }

            var queryString = Resources.GetSparql(key);
            var query = new SparqlParameterizedString(queryString);

            query.SetUri("schemaUri", new Uri("http://example.com"));
            query.SetUri("instanceUri", new Uri("http://example.com"));

            OpenApiParameter[] parameters = Resources.GetSparqlParameters(endpoint).ToArray();
            if (parameters.Any())
            {
                var values = parameters.ToDictionary(
                    parameter => parameter.Name,
                    parameter =>
                    {
                        switch (Resources.GetParameterType(parameter))
                        {
                            case ParameterType.Uri:
                                return "http://example.com";
                            case ParameterType.Literal:
                                if (parameter?.Example?.GetType() == typeof(Microsoft.OpenApi.Any.OpenApiInteger))
                                    return "0";
                                else
                                    return "EXAMPLE";
                            default:
                                return "EXAMPLE";
                        }
                    });

                FixedQueryController.SetParameters(query, parameters, values);
            }

            var validator = new SparqlQueryValidator();
            var result = validator.Validate(query.ToString());

            Assert.IsTrue(result.IsValid, result.Message);
        }
    }
}