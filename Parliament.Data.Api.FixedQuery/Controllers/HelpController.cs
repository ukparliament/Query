namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;
    using System.Web.Http;

    [RoutePrefix("")]
    public class IndexController : ApiController
    {
        [Route()]
        public HttpResponseMessage Get()
        {
            var links = new string[] {
                this.Url.Route("PersonById", new { id = "01679448-ebe8-4263-87de-a749a937239c" }),
                this.Url.Route("PersonByInitial", new { initial = "ö" }),
                this.Url.Route("PersonLookup", new { source = "mnisId", id = "3299" }),
                this.Url.Route("PersonByLetters", new { letters = "ee" }),

                this.Url.Route("Member", null),
                this.Url.Route("MemberCurrent", null),
                this.Url.Route("MemberByInitial", new { initial = "y" }),
                this.Url.Route("MemberCurrentByInitial", new { initial = "z" }),

                this.Url.Route("ConstituencyByID", new { id = "dcd95ee4-b11c-4b1a-9b7e-70a724b7daf8" }),
                this.Url.Route("ConstituencyByInitial", new { initial = "l" }),
                this.Url.Route("ConstituencyCurrent", null),
                this.Url.Route("ConstituencyLookup", new { source = "onsCode", id = "E14000699" }),
                this.Url.Route("ConstituencyByLetters", new { letters = "heath" }),
                this.Url.Route("ConstituencyCurrentByInitial", new { initial = "v" }),

                this.Url.Route("PartyById", new { id = "dcd95ee4-b11c-4b1a-9b7e-70a724b7daf8" }),
                this.Url.Route("PartyByInitial", new { initial = "a" }),
                this.Url.Route("PartyCurrent", null),
                this.Url.Route("PartyByLetters", new { letters = "lab" }),

            };

            var response = Request.CreateResponse();

            response.Content = new StringContent($@"<!DOCTYPE html>
<html>
    <body>
        <ul>{string.Join(string.Empty, from link in links select $"<li><a href='{link}'>{HttpUtility.UrlDecode(link).Substring(this.Configuration.VirtualPathRoot.Length)}</a>")}</ul>
      </body>
  </html>");

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}