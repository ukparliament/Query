namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Web.Http.Controllers;
    using VDS.RDF;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FixedQueryControllerConfigurationAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Services.Replace(typeof(IContentNegotiator), new DefaultContentNegotiator(true));

            var definitions = MimeTypesHelper.Definitions.Where(definition => definition.CanWriteRdf || definition.CanWriteSparqlResults || definition.CanWriteRdfDatasets);

            var formatters = definitions
                // First add all the extension mappings.
                .SelectMany(definition => definition.FileExtensions
                    .Where(extension => extension == definition.CanonicalFileExtension)
                    .Where(extension => !extension.Contains("."))
                .Select(extension => new UriPathExtensionMapping(extension, definition.CanonicalMimeType) as MediaTypeMapping))
            // Then add the query string mappings.
            .Union(definitions
                .Select(definition => definition.CanonicalMimeType)
                .Distinct()
                .Select(definition => new QueryStringMapping("format", definition, definition) as MediaTypeMapping))
            // Finally add the accept header mappings.
            .Union(definitions
                .Select(definition => definition.CanonicalMimeType)
                .Distinct()
                .Select(definition => new RequestHeaderMapping("Accept", definition, StringComparison.OrdinalIgnoreCase, false, definition) as MediaTypeMapping))
            .Select(mapping => new GraphFormatter(mapping));

            controllerSettings.Formatters.AddRange(formatters);
        }
    }
}
