// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Parliament.Data.Api.FixedQuery
{
    using Microsoft.OpenApi.Any;
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

        public static string OpenApiDefinitionResourceName
        {
            get
            {
                return $"{BaseName}.OpenApiDefinition.json";
            }
        }

        private static OpenApiDocument openApiDefinition = null;
        public static OpenApiDocument OpenApiDefinition
        {
            get
            {
                if (openApiDefinition == null)
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(OpenApiDefinitionResourceName))
                    {
                        apiDiagnostic = new OpenApiDiagnostic();
                        openApiDefinition = new OpenApiStreamReader().Read(stream, out apiDiagnostic);
                    }
                return openApiDefinition;
            }
        }

        private static OpenApiDiagnostic apiDiagnostic = null;
        public static OpenApiDiagnostic ApiDiagnostic
        {
            get
            {
                return apiDiagnostic;
            }
        }


        public static OpenApiPathItem GetApiPathItem(string endpointName)
        {
            string key = $"/{endpointName}{{ext}}";
            if (OpenApiDefinition.Paths.ContainsKey(key))
                return OpenApiDefinition.Paths[key];
            else
                return null;
        }

        public static EndpointType GetEndpointType(OpenApiPathItem openApiPath)
        {
            string xType = ((OpenApiString)openApiPath.Extensions["x-type"]).Value;
            return (EndpointType)Enum.Parse(typeof(EndpointType), xType, true);
        }

        public static ParameterType GetParameterType(OpenApiParameter openApiParameter)
        {
            string xType = ((OpenApiString)openApiParameter.Extensions["x-type"]).Value;
            return (ParameterType)Enum.Parse(typeof(ParameterType), xType, true);
        }

        public static Uri GetLiteralParameterType(OpenApiParameter openApiParameter)
        {
            if ((openApiParameter.Extensions.ContainsKey("x-literal-type")) &&
                (Uri.TryCreate(((OpenApiString)openApiParameter.Extensions["x-literal-type"])?.Value, UriKind.Absolute, out Uri literalKind)))
                return literalKind;
            else
                return null;
        }
        public static IEnumerable<OpenApiParameter> GetSparqlParameters(OpenApiPathItem openApiPath)
        {
            return openApiPath.Operations[OperationType.Get].Parameters
                .Where(p => p.Name != "ext" && p.Name != "format");
        }

        public static string GetOpenApiDefinition()
        {
            return Resources.GetFile(OpenApiDefinitionResourceName);
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