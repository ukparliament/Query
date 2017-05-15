namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("a2a84c18-75f6-460e-b690-eb6cbd358312")]
    public class SparqlController : BaseController
    {
        [Route()]
        public Graph Post(QueryWrapper wrapper)
        {
            var queryString = wrapper.Query;

            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("query missing") });
            }

            return BaseController.ExecuteList(new SparqlParameterizedString(queryString));
        }

        [Route()]
        public HttpResponseMessage Get(string query = "")
        {
            var response = Request.CreateResponse();

            response.Content = new StringContent($@"
<!DOCTYPE html>
<html>
    <head>
        <meta charset='utf-8'>
        <style>
            html, body {{height: 100%;}}
            body {{margin: 0; display: flex; flex-direction: column;}}
            textarea {{width: 99%; max-width: 99%; height: 100px;}}
            iframe {{border: 0; min-height: 0; flex-grow: 1;}}
            button {{width: 100%;}}
        </style>
    </head>
    <body>
        <div>
            <form method='post' target='results'>
                <textarea name='query' required>PREFIX : <http://id.parliament.uk/schema/>
PREFIX id: <http://id.parliament.uk/>

CONSTRUCT {{?s ?p ?o}}
WHERE {{?s ?p ?o}}
LIMIT 1</textarea>
                <button accesskey='e'>Execute</button>
            </form>
        </div>
        <iframe name='results'></iframe>
    </body>
</html>
");

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }

    public class QueryWrapper
    {
        [HttpBindRequired]
        public string Query { get; set; }
    }
}