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

            var definitions = MimeTypesHelper.Definitions.Where(x => x.CanWriteRdf);
            var mappings =
                // First add all the extension mappings.
                definitions
                    .SelectMany(definition => definition.FileExtensions
                        .Select(extension => new UriPathExtensionMapping(extension, definition.CanonicalMimeType) as MediaTypeMapping))
                // Then add the query string mappings.
                .Union(definitions
                    .SelectMany(definition => definition.MimeTypes
                        .Select(mimeType => new QueryStringMapping("format", mimeType, definition.CanonicalMimeType) as MediaTypeMapping)))
                // Finally add the accept header mappings.
                .Union(definitions
                    .SelectMany(definition => definition.MimeTypes
                        .Select(mimeType => new RequestHeaderMapping("Accept", mimeType, StringComparison.OrdinalIgnoreCase, false, definition.CanonicalMimeType) as MediaTypeMapping)))
                .Select(mapping => new GraphFormatter(mapping));

            controllerSettings.Formatters.AddRange(mappings);
        }
    }
}
