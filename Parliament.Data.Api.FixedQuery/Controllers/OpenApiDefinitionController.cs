using Microsoft.OpenApi.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http.Controllers;

namespace Parliament.Data.Api.FixedQuery.Controllers
{
    public class OpenApiDefinitionController : BaseController
    {
        public HttpResponseMessage Get()
        {
            string definition = Resources.GetOpenApiDefinition();

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(definition, System.Text.Encoding.UTF8, "application/json");

            return response;
        }
    }
}