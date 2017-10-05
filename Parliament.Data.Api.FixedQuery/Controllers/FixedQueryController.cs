namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using Microsoft.ApplicationInsights;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using VDS.RDF;
    using VDS.RDF.Query;

    // TODO: Merge with BaseController
    [FixedQueryControllerConfiguration]
    public class FixedQueryController : BaseController
    {
        public HttpResponseMessage Get(string name)
        {
            var parameters = this.Request.GetQueryNameValuePairs().ToDictionary(parameter => parameter.Key, parameter => parameter.Value);

            return this.Get(name, parameters);
        }

        internal HttpResponseMessage Get(string name, Dictionary<string, string> values)
        {
            new TelemetryClient().TrackEvent(name, values);

            var endpoint = Resources.DB.Endpoints[name];

            if (endpoint.Type == EndpointType.HardCoded)
            {
                return CreateHardcodedResponse(name, values);
            }

            var queryString = Resources.GetSparql(name);
            var query = new SparqlParameterizedString(queryString);

            query.SetUri("schemaUri", Schema);

            if (endpoint.Parameters != null)
            {
                FixedQueryController.SetParameters(query, endpoint.Parameters, values);
            }

            var result = ExecuteQuery(query, endpoint.Type);

            return CreateResponse(result);
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

        private HttpResponseMessage CreateHardcodedResponse(string name, Dictionary<string, string> values)
        {
            var method = typeof(HardCoded).GetMethod(name, BindingFlags.Public | BindingFlags.Static);
            var result = method.Invoke(null, values.Values.Cast<object>().ToArray()) as Graph;

            return this.Request.CreateResponse<IGraph>(result);
        }

        public static void SetParameters(SparqlParameterizedString query, Dictionary<string, ParameterType> parameters, Dictionary<string, string> values)
        {
            foreach (var parameterDefinition in parameters)
            {
                var name = parameterDefinition.Key;
                if (!values.ContainsKey(name))
                {
                    throw new Exception($"missing parameter {name}");
                }

                var value = values[name];
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception($"missing value for {name}");
                }

                var type = parameterDefinition.Value;

                switch (type)
                {
                    case ParameterType.Uri:
                        query.SetUri(name, new Uri(value));
                        break;

                    case ParameterType.InstanceUri:
                        query.SetUri(name, new Uri(BaseController.Instance, value));
                        break;

                    case ParameterType.SchemaUri:
                        query.SetUri(name, new Uri(BaseController.Schema, value));
                        break;

                    case ParameterType.Literal:
                        query.SetLiteral(name, value);
                        break;
                }
            }
        }
    }
}