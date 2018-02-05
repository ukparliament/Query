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

            var definitions = Global.MimeTypeDefinitions;

            var formatters = definitions
                // First add all the extension mappings.
                .Where(definition => definition.Extensions != null)
                .SelectMany(definition => definition.Extensions.Select(extension => new { extension = extension, canonical = definition.MimeTypes.First() }))
                .Select(extension => new UriPathExtensionMapping(extension.extension, extension.canonical) as MediaTypeMapping)
            // Then add the query string mappings.
            .Union(definitions
                .SelectMany(definition => definition.MimeTypes.Select(mimeType => new { mimeType = mimeType, canonical = definition.MimeTypes.First() }))
                .Select(definition => new QueryStringMapping("format", definition.mimeType, definition.canonical) as MediaTypeMapping))
            // Finally add the accept header mappings.
            .Union(definitions
                .SelectMany(definition => definition.MimeTypes.Select(mimeType => new { mimeType = mimeType, canonical = definition.MimeTypes.First() }))
                .Select(definition => new RequestHeaderMapping("Accept", definition.mimeType, StringComparison.OrdinalIgnoreCase, false, definition.canonical) as MediaTypeMapping))
            .Select(mapping => new GraphFormatter(mapping));

            controllerSettings.Formatters.AddRange(formatters);
        }
    }
}
