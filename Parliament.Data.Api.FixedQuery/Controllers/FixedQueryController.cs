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
    using Microsoft.ApplicationInsights;
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Http.Description;
    using VDS.RDF;
    using VDS.RDF.Query;
    using VDS.RDF.Writing.Formatting;

    // TODO: Merge with BaseController
    [FixedQueryControllerConfiguration]
    public class FixedQueryController : BaseController
    {
        public HttpResponseMessage Get(string name)
        {
            var parameters = this.Request.GetQueryNameValuePairs().ToDictionary(parameter => parameter.Key, parameter => parameter.Value);

            new TelemetryClient().TrackEvent(name, parameters);

            var result = FixedQueryController.Execute(name, parameters);

            return this.CreateResponse(result);
        }

        private static object Execute(string name, Dictionary<string, string> values)
        {
            var endpoint = Resources.GetApiPathItem(name);
            var endpointType = Resources.GetEndpointType(endpoint);

            if (endpointType == EndpointType.HardCoded)
            {
                return FixedQueryController.ExecuteHardCoded(name, values);
            }
            else
            {
                return FixedQueryController.ExecuteNamedSparql(name, values);
            }
        }

        internal static object ExecuteNamedSparql(string name, Dictionary<string, string> values)
        {
            var endpoint = Resources.GetApiPathItem(name);
            var queryString = Resources.GetSparql(name);
            var query = new SparqlParameterizedString(queryString);
            var endpointType = Resources.GetEndpointType(endpoint);

            query.SetUri("schemaUri", Global.SchemaUri);
            query.SetUri("instanceUri", Global.InstanceUri);


            var parameters = Resources.GetSparqlParameters(endpoint);
            if ((parameters != null) && (parameters.Any()))
            {
                FixedQueryController.SetParameters(query, parameters, values);
            }

            return FixedQueryController.ExecuteQuery(query, endpointType);
        }

        private static object ExecuteQuery(SparqlParameterizedString query, EndpointType type)
        {
            switch (type)
            {
                case EndpointType.Single:
                    return BaseController.ExecuteSingle(query);

                case EndpointType.List:
                    return BaseController.ExecuteList(query);

                default:
                    throw new Exception($"unknown query type {type}");
            }
        }

        private HttpResponseMessage CreateResponse(object result)
        {
            if (result is IGraph)
            {
                return this.Request.CreateResponse<IGraph>(result as IGraph);
            }
            else
            {
                return this.Request.CreateResponse<SparqlResultSet>(result as SparqlResultSet);
            }
        }

        private static object ExecuteHardCoded(string name, Dictionary<string, string> values)
        {
            var method = typeof(HardCoded).GetMethod(name, BindingFlags.Public | BindingFlags.Static);

            try
            {
                return method.Invoke(null, new object[] { values }) as Graph;
            }
            catch (TargetInvocationException e) when (e.InnerException is HttpResponseException)
            {
                throw e.InnerException;
            }
        }

        public static void SetParameters(SparqlParameterizedString query, IEnumerable<OpenApiParameter> parameters, Dictionary<string, string> values)
        {
            foreach (var parameterDefinition in parameters)
            {
                var name = parameterDefinition.Name;
                if (!values.ContainsKey(name))
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent($"missing parameter {name}") });
                }

                var value = values[name];
                if (string.IsNullOrEmpty(value))
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent($"missing value for parameter {name}") });
                }

                var type = Resources.GetParameterType(parameterDefinition);

                switch (type)
                {
                    case ParameterType.Uri:
                        query.SetUri(name, new Uri(value));
                        break;

                    case ParameterType.InstanceUri:
                        query.SetUri(name, new Uri(Global.InstanceUri, value));
                        break;

                    case ParameterType.InstanceUris:
                        FixedQueryController.SetUris(query, name, value, Global.InstanceUri);
                        break;

                    case ParameterType.SchemaUri:
                        query.SetUri(name, new Uri(Global.SchemaUri, value));
                        break;

                    case ParameterType.SchemaUris:
                        FixedQueryController.SetUris(query, name, value, Global.SchemaUri);
                        break;

                    case ParameterType.Literal:
                        Uri literalKind = Resources.GetLiteralParameterType(parameterDefinition);
                        if (literalKind != null)
                            query.SetLiteral(name, value, literalKind);
                        else
                            query.SetLiteral(name, value);
                        break;
                }
            }
        }

        private static void SetUris(SparqlParameterizedString query, string name, string value, Uri baseUri)
        {
            var formatter = new SparqlFormatter();
            var factory = new NodeFactory();

            Func<string, string> format = instanceId =>
            {
                var uri = new Uri(baseUri, instanceId);
                var node = factory.CreateUriNode(uri);

                return formatter.Format(node);
            };

            var formattedUris = value.Split(',').Select(format);

            value = string.Join(" ", formattedUris);

            var parameter = $"@{name}";

            query.CommandText = query.CommandText.Replace(parameter, value);
        }
    }
}