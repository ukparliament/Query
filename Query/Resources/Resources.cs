namespace Query
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Exceptions;
    using Microsoft.OpenApi.Interfaces;
    using Microsoft.OpenApi.Models;
    using Microsoft.OpenApi.Readers;

    public static class Resources
    {
        private const string BaseName = "Query.Resources";
        private static OpenApiDocument openApiDocument;

        public static OpenApiDocument OpenApiDocument
        {
            get
            {
                if (Resources.openApiDocument is null)
                {
                    using (var stream = Resources.GetStream($"{BaseName}.OpenApiDocumentTemplate.json"))
                    {
                        var reader = new OpenApiStreamReader();
                        var document = reader.Read(stream, out var diagnostic);

                        if (diagnostic.Errors.Any())
                        {
                            throw new OpenApiException(diagnostic.Errors.First().Message);
                        }

                        var graphMappings = Configuration.QueryMappings.Where(m => !(m.rdfWriter is null) || !(m.storeWriter is null));
                        var nonGraphMappings = Configuration.QueryMappings.Where(m => !(m.sparqlWriter is null));

                        document.Components.Responses["graphResponse"].Content = graphMappings.SelectMany(m => m.MediaTypes).ToDictionary(m => m, m => new OpenApiMediaType());
                        document.Components.Responses["nonGraphResponse"].Content = nonGraphMappings.SelectMany(m => m.MediaTypes).ToDictionary(m => m, m => new OpenApiMediaType());
                        document.Components.Parameters["formatGraph"].Schema.Enum = graphMappings.SelectMany(m => m.MediaTypes.Select(e => new OpenApiString(e) as IOpenApiAny)).ToList();
                        document.Components.Parameters["formatNonGraph"].Schema.Enum = nonGraphMappings.SelectMany(m => m.MediaTypes.Select(e => new OpenApiString(e) as IOpenApiAny)).ToList();
                        document.Components.Parameters["fileExtensionGraph"].Schema.Enum = graphMappings.SelectMany(m => m.Extensions.Select(e => new OpenApiString($".{e}") as IOpenApiAny)).ToList();
                        document.Components.Parameters["fileExtensionNonGraph"].Schema.Enum = nonGraphMappings.SelectMany(m => m.Extensions.Select(e => new OpenApiString($".{e}") as IOpenApiAny)).ToList();

                        Resources.openApiDocument = document;
                    }
                }

                return Resources.openApiDocument;
            }
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

        public static TEnum GetXType<TEnum>(IOpenApiExtensible path) where TEnum : struct
        {
            var type = ((OpenApiString)path.Extensions["x-type"]).Value;
            return Enum.Parse<TEnum>(type, true);
        }

        public static string GetSparql(string name)
        {
            return Resources.GetFile($"{BaseName}.Sparql.{name}.sparql");
        }

        public static IEnumerable<OpenApiParameter> GetSparqlParameters(OpenApiPathItem path)
        {
            return path.Operations[OperationType.Get].Parameters
                .Where(p => p.Name != "ext" && p.Name != "format");
        }

        private static Stream GetStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }

        private static string GetFile(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
