namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Web.Http.Controllers;
    using VDS.RDF;

    public class FixedQueryControllerConfigurationAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Services.Replace(typeof(IContentNegotiator), new DefaultContentNegotiator(true));

            var definitions = MimeTypesHelper.Definitions.Where(definition => definition.CanWriteRdf);
            
            var mappings =
                // First add all the extension mappings.
                definitions
                    .SelectMany(definition => definition.FileExtensions.Where(extension => !extension.Contains("."))
                        .Select(extension => new UriPathExtensionMapping(extension, definition.CanonicalMimeType) as MediaTypeMapping))
                // Then add the query string mappings.
                .Union(definitions
                    .Select(x => x.CanonicalMimeType).Distinct()
                    .Select(definition => new QueryStringMapping("format", definition, definition) as MediaTypeMapping))
                // Finally add the accept header mappings.
                .Union(definitions
                    .Select(x => x.CanonicalMimeType).Distinct()
                    .Select(definition => new RequestHeaderMapping("Accept", definition, StringComparison.OrdinalIgnoreCase, false, definition) as MediaTypeMapping))
                .Select(mapping => new GraphFormatter(mapping));

            controllerSettings.Formatters.AddRange(mappings);
        }
    }
}
