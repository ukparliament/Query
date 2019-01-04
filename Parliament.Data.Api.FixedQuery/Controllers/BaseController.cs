// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Configuration;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;
    using VDS.RDF;
    using VDS.RDF.Query;
    using VDS.RDF.Storage;

    // TODO: Merge with FixedQueryController
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract partial class BaseController : ApiController
    {
        protected static object ExecuteSingle(SparqlParameterizedString query, string endpointUri = null)
        {
            var result = ExecuteList(query, endpointUri);

            if (result is IGraph graph && graph.IsEmpty || result is SparqlResultSet resultSet && resultSet.IsEmpty)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return result;
        }

        protected static object ExecuteList(SparqlParameterizedString query, string endpointUri = null)
        {
            // TODO: This should move to controller action
            var queryString = query.ToString();

            SparqlRemoteEndpoint endpoint = null;
            if (string.IsNullOrWhiteSpace(endpointUri))
                endpoint = new GraphDBSparqlEndpoint();
            else
                endpoint = new SparqlRemoteEndpoint(new Uri(endpointUri));

            using (var connector = new SparqlConnector(endpoint))
            {
                var results = connector.Query(queryString);

                if (results is IGraph)
                {
                    AddNamespaces(results as IGraph);
                }

                return results;
            }
        }

        private static void AddNamespaces(IGraph graph)
        {
            graph.NamespaceMap.AddNamespace("owl", new Uri("http://www.w3.org/2002/07/owl#"));
            graph.NamespaceMap.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            graph.NamespaceMap.AddNamespace("id", Global.InstanceUri);
            graph.NamespaceMap.AddNamespace("schema", Global.SchemaUri);
        }
    }
}