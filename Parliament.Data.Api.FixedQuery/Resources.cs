namespace Parliament.Data.Api.FixedQuery
{
    using Microsoft.OpenApi.Models;
    using Microsoft.OpenApi.Readers;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static class Resources
    {
        private const string BaseName = "Parliament.Data.Api.FixedQuery";

        private static DB db;

        public static DB DB
        {
            get
            {
                if (Resources.db == null)
                {
                    Resources.db = JsonConvert.DeserializeObject<DB>(Resources.EndpointsJson);
                }

                return Resources.db;
            }
        }

        public static string EndpointsJson
        {
            get
            {
                return Resources.GetFile($"{BaseName}.Endpoints.json");
            }
        }

        public static string EndpointsSchema
        {
            get
            {
                return Resources.GetFile($"{BaseName}.EndpointsSchema.json");
            }
        }

        private static OpenApiDocument openApiDefinition = null;
        public static OpenApiDocument OpenApiDefinition
        {
            get
            {
                if (openApiDefinition == null)
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{BaseName}.openapi.json"))
                    {
                        openApiDefinition = new OpenApiStreamReader().Read(stream, out OpenApiDiagnostic diagnostic);
                    }
                return openApiDefinition;
            }
        }

        public static string GetSparql(string name)
        {
            return Resources.GetFile($"{BaseName}.Sparql.{name}.sparql");
        }

        public static IEnumerable<string> SparqlFileNames
        {
            get
            {
                var prefix = $"{BaseName}.Sparql.";
                var suffix = ".sparql";

                return Assembly.GetExecutingAssembly()
                    .GetManifestResourceNames()
                    .Where(resourceName => resourceName.StartsWith(prefix))
                    .Where(resourceName => resourceName.EndsWith(suffix))
                    .Select(resourceName => resourceName.Split(new[] { prefix, suffix }, StringSplitOptions.None))
                    .Select(components => components[1])
                    .Except(new[] { "constituency_lookup_by_postcode-external" });
            }
        }

        private static string GetFile(string resourceName)
        {
            using (var sparqlResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(sparqlResourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}