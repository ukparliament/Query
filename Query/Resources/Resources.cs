namespace Query
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Exceptions;
    using Microsoft.OpenApi.Models;
    using Microsoft.OpenApi.Readers;

    public static class Resources
    {
        public static OpenApiDocument OpenApiDocument
        {
            get
            {
                using (var stream = Resources.GetStream("Query.Resources.OpenApiDocumentTemplate.json"))
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

                    return document;
                }
            }
        }

        public static Stream GetStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }
    }
}
