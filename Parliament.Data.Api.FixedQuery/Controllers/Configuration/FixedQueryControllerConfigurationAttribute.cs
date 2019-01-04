// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
                .SelectMany(definition => definition.Extensions.Select(extension => new { extension, canonical = definition.MimeTypes.First() }))
                .Select(extension => new UriPathExtensionMapping(extension.extension, extension.canonical) as MediaTypeMapping)
            // Then add the query string mappings.
            .Union(definitions
                .SelectMany(definition => definition.MimeTypes.Select(mimeType => new { mimeType, canonical = definition.MimeTypes.First() }))
                .Select(definition => new QueryStringMapping("format", definition.mimeType, definition.canonical) as MediaTypeMapping))
            // Finally add the accept header mappings.
            .Union(definitions
                .SelectMany(definition => definition.MimeTypes.Select(mimeType => new { mimeType, canonical = definition.MimeTypes.First() }))
                .Select(definition => new RequestHeaderMapping("Accept", definition.mimeType, StringComparison.OrdinalIgnoreCase, false, definition.canonical) as MediaTypeMapping))
            .Select(mapping => new GraphFormatter(mapping));

            controllerSettings.Formatters.AddRange(formatters);
        }
    }
}
