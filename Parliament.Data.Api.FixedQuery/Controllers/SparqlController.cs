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
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http;
    using System.Web.Http.Description;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("sparql")]
    public class SparqlController : BaseController
    {
        [Route]
        public object Post(QueryWrapper wrapper)
        {
            if (wrapper == null || string.IsNullOrWhiteSpace(wrapper.Query))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("query missing") });
            }

            var queryString = wrapper.Query;

            return BaseController.ExecuteList(new SparqlParameterizedString(queryString));
        }

        [Route]
        public HttpResponseMessage Get()
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
}