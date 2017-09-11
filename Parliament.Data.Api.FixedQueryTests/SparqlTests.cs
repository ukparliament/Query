namespace Parliament.Data.Api.FixedQuery.Tests
{
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
                return Resources.DB.Endpoints.Select(endpoint => new object[] { endpoint.Key });
            }
        }

        [TestMethod]
        [DynamicData("Endpoints")]
        public void ValidateSparql(string endpointName)
        {
            var endpoint = Resources.DB.Endpoints[endpointName];
            if (endpoint.Type == EndpointType.HardCoded)
            {
                return;
            }

            var queryString = Resources.GetSparql(endpointName);
            var query = new SparqlParameterizedString(queryString);

            query.SetUri("schemaUri", new Uri("http://example.com"));

            if (endpoint.Parameters != null)
            {
                var values = endpoint.Parameters.ToDictionary(
                    parameter => parameter.Key,
                    parameter =>
                    {
                        switch (parameter.Value)
                        {
                            case ParameterType.Uri:
                                return "http://example.com";
                            default:
                                return "EXAMPLE";
                        }
                    });

                FixedQueryController.SetParameters(query, endpoint.Parameters, values);
            }

            var validator = new SparqlQueryValidator();
            var result = validator.Validate(query.ToString());

            Assert.IsTrue(result.IsValid, result.Message);
        }
    }
}