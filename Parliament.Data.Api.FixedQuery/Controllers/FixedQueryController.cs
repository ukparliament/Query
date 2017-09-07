namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using VDS.RDF;
    using VDS.RDF.Query;

    public class FixedQueryController : BaseController
    {
        public Graph Get(string name)
        {
            var parameters = this.Request.GetQueryNameValuePairs().ToDictionary(parameter => parameter.Key, parameter => parameter.Value);
            return FixedQueryController.Get(name, parameters);
        }

        internal static Graph Get(string name, Dictionary<string, string> values)
        {
            var endpoint = Resources.DB.Endpoints[name];
            if (endpoint.Type == EndpointType.HardCoded)
            {
                var method = typeof(HardCoded).GetMethod(name, BindingFlags.Public | BindingFlags.Static);
                return method.Invoke(null, values.Values.Cast<object>().ToArray()) as Graph;
            }


            var queryString = Resources.GetSparql(name);
            var query = new SparqlParameterizedString(queryString);

            if (endpoint.Parameters != null)
            {
                FixedQueryController.SetParameters(query, endpoint.Parameters, values);
            }

            switch (endpoint.Type)
            {
                case EndpointType.Single:
                    return BaseController.ExecuteSingle(query);

                case EndpointType.List:
                    return BaseController.ExecuteList(query);

                default:
                    throw new Exception($"unknown query type {endpoint.Type}");
            }
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
                        query.SetUri(name, new Uri(BaseController.instance, value));
                        break;

                    case ParameterType.SchemaUri:
                        query.SetUri(name, new Uri(BaseController.schema, value));
                        break;

                    case ParameterType.Literal:
                        query.SetLiteral(name, value);
                        break;
                }
            }
        }
    }
}