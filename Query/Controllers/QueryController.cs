namespace Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.OpenApi.Models;
    using VDS.RDF;
    using VDS.RDF.Query;
    using VDS.RDF.Storage;
    using VDS.RDF.Writing.Formatting;

    public class QueryController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public QueryController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("{name}")]
        [HttpGet("{name}.{format:query}")]
        [FormatFilter]
        public object Get(string name)
        {
            var parameters = this.Request.Query.ToDictionary(parameter => parameter.Key, parameter => parameter.Value.ToString());

            return Execute(name, parameters);
        }

        private object Execute(string name, Dictionary<string, string> values)
        {
            var endpoint = Resources.OpenApiDocument.Paths[$"/{name}{{ext}}"];
            var endpointType = Resources.GetXType<EndpointType>(endpoint);

            if (endpointType == EndpointType.HardCoded)
            {
                return this.ExecuteHardCoded(name, values);
            }
            else
            {
                return ExecuteNamedSparql(name, values);
            }
        }

        internal static object ExecuteNamedSparql(string name, Dictionary<string, string> values)
        {
            var endpoint = Resources.OpenApiDocument.Paths[$"/{name}{{ext}}"];
            var queryString = Resources.GetSparql(name);
            var query = new SparqlParameterizedString(queryString);
            var endpointType = Resources.GetXType<EndpointType>(endpoint);

            query.SetUri("schemaUri", Configuration.SchemaUri);

            var parameters = Resources.GetSparqlParameters(endpoint);
            if ((parameters != null) && (parameters.Any()))
            {
                SetParameters(query, parameters, values);
            }

            return ExecuteQuery(query, endpointType);
        }

        private static object ExecuteQuery(SparqlParameterizedString query, EndpointType type)
        {
            switch (type)
            {
                case EndpointType.Single:
                    return ExecuteSingle(query);

                case EndpointType.List:
                    return ExecuteList(query);

                default:
                    throw new Exception($"unknown query type {type}");
            }
        }

        private IActionResult ExecuteHardCoded(string name, Dictionary<string, string> values)
        {
            var method = typeof(HardCoded).GetMethod(name, BindingFlags.Public | BindingFlags.Static);

            return (IActionResult)method.Invoke(null, new object[] { values });
        }

        public static void SetParameters(SparqlParameterizedString query, IEnumerable<OpenApiParameter> parameters, Dictionary<string, string> values)
        {
            foreach (var parameterDefinition in parameters)
            {
                var name = parameterDefinition.Name;
                if (!values.ContainsKey(name))
                {
                    throw new Exception($"missing parameter {name}");
                }

                var value = values[name];
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception($"missing value for parameter {name}");
                }

                var type = Resources.GetXType<ParameterType>(parameterDefinition);

                switch (type)
                {
                    case ParameterType.Uri:
                        query.SetUri(name, new Uri(value));
                        break;

                    case ParameterType.InstanceUri:
                        query.SetUri(name, new Uri(Configuration.InstanceUri, value));
                        break;

                    case ParameterType.InstanceUris:
                        QueryController.SetUris(query, name, value);
                        break;

                    case ParameterType.SchemaUri:
                        query.SetUri(name, new Uri(Configuration.SchemaUri, value));
                        break;

                    case ParameterType.Literal:
                        query.SetLiteral(name, value);
                        break;
                }
            }
        }

        private static void SetUris(SparqlParameterizedString query, string name, string value)
        {
            var formatter = new SparqlFormatter();
            var factory = new NodeFactory();

            Func<string, string> format = instanceId =>
            {
                var uri = new Uri(Configuration.InstanceUri, instanceId);
                var node = factory.CreateUriNode(uri);

                return formatter.Format(node);
            };

            var formattedUris = value.Split(',').Select(format);

            value = string.Join(" ", formattedUris);

            var parameter = $"@{name}";

            query.CommandText = query.CommandText.Replace(parameter, value);
        }

        internal static object ExecuteSingle(SparqlParameterizedString query, string endpointUri = null)
        {
            var result = ExecuteList(query, endpointUri);

            if (result is IGraph graph && graph.IsEmpty || result is SparqlResultSet resultSet && resultSet.IsEmpty)
            {
                return new NotFoundResult();
            }

            return result;
        }

        internal static object ExecuteList(SparqlParameterizedString query, string endpointUri = null)
        {
            // TODO: This should move to controller action
            var queryString = query.ToString();

            var endpoint = (SparqlRemoteEndpoint)null;
            if (string.IsNullOrWhiteSpace(endpointUri))
            {
                endpoint = new GraphDBSparqlEndpoint(this.configuration);
            }
            else
            {
                endpoint = new SparqlRemoteEndpoint(new Uri(endpointUri));
            }

            using (var connector = new SparqlConnector(endpoint))
            {
                var results = connector.Query(queryString);

                if (results is IGraph resultGraph)
                {
                    AddNamespaces(resultGraph);
                }

                return results;
            }
        }

        private static void AddNamespaces(IGraph graph)
        {
            graph.NamespaceMap.AddNamespace("owl", new Uri("http://www.w3.org/2002/07/owl#"));
            graph.NamespaceMap.AddNamespace("rdf", new Uri("http://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            graph.NamespaceMap.AddNamespace("id", Configuration.InstanceUri);
            graph.NamespaceMap.AddNamespace("schema", Configuration.SchemaUri);
        }
    }
}
