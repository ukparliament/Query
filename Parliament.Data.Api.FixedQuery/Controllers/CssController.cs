namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    public partial class CssController : BaseController
    {
        [HttpGet]
        public HttpResponseMessage ResourceStyle()
        {        

            var response = Request.CreateResponse();

            response.Content = new StringContent(@"
h3 {
    display: none;
    }

table {
    border-collapse: collapse;
}

td {
    border: 1px solid;
    margin: 0;
    padding: 0;
}
");

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/css");

            return response;
        }
    }
}
