using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace Parliament.Data.Api.FixedQuery
{
    public class FixedQueryDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            Uri serverUrl = new Uri(Resources.OpenApiDefinition.Servers.SingleOrDefault().Url);
            swaggerDoc.basePath = serverUrl.PathAndQuery;
            swaggerDoc.host = serverUrl.Host;
            swaggerDoc.schemes = new string[] { serverUrl.Scheme };
            swaggerDoc.info.title = Resources.OpenApiDefinition.Info.Title;
            swaggerDoc.info.version = Resources.OpenApiDefinition.Info.Version;

            swaggerDoc.paths.Clear();
            foreach (KeyValuePair<string, OpenApiPathItem> path in Resources.OpenApiDefinition.Paths)
            {
                swaggerDoc.paths.Add(new KeyValuePair<string, PathItem>(path.Key, new PathItem()
                {
                    get = new Operation()
                    {
                        parameters = path.Value.Operations.SingleOrDefault().Value.Parameters.Select(p => new Parameter()
                        {
                            @in = p.In.Value.ToString().ToLower(),
                            required = p.Required,
                            name = p.Name,
                            description = p.Description,
                            type = p.Schema.Type,
                        }).ToList(),
                        responses = path.Value.Operations.SingleOrDefault().Value.Responses.Select(r => new KeyValuePair<string, Response>(
                            r.Key, new Response()
                            {
                                description = r.Value.Description,
                            })).ToDictionary(kv => kv.Key, kv => kv.Value),
                        produces = path.Value.Operations.SingleOrDefault().Value.Responses.SelectMany(r => r.Value.Content.Select(c => c.Key)).ToList()
                    }
                }));
            }
        }
    }
}