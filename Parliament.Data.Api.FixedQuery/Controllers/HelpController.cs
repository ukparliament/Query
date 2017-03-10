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
                this.Url.Route("PersonIndex", null),
                this.Url.Route("PersonById", new { id = "f3b5d9a2-88e6-45ba-96ef-17766b1acdcc" }),
                this.Url.Route("PersonByInitial", new { initial = "ö" }),
                this.Url.Route("PersonLookup", new { source = "mnisId", id = "3299" }),
                this.Url.Route("PersonByLetters", new { letters = "ee" }),
                this.Url.Route("PersonAToZ", null),
                this.Url.Route("PersonConstituencies", new { id = "f3b5d9a2-88e6-45ba-96ef-17766b1acdcc" }),
                this.Url.Route("PersonCurrentConstituency", new { id = "f3b5d9a2-88e6-45ba-96ef-17766b1acdcc" }),
                this.Url.Route("PersonParties", new { id = "a9bd4923-9964-4196-b6b7-e42ada6e5284" }),
                this.Url.Route("PersonCurrentParty", new { id = "a9bd4923-9964-4196-b6b7-e42ada6e5284" }),
                this.Url.Route("PersonContactPoints", new { id = "c6c0bfc6-bf28-4958-9cb2-939ea0e86c95" }),
                this.Url.Route("PersonHouses", new { id = "c6c0bfc6-bf28-4958-9cb2-939ea0e86c95" }),
                this.Url.Route("PersonCurrentHouse", new { id = "c6c0bfc6-bf28-4958-9cb2-939ea0e86c95" }),


                // Member route exists on person controller
                this.Url.Route("Member", null),

                this.Url.Route("MemberCurrent", null),
                this.Url.Route("MemberByInitial", new { initial = "y" }),
                this.Url.Route("MemberCurrentByInitial", new { initial = "z" }),
                this.Url.Route("MemberAToZ", null),
                this.Url.Route("MemberCurrentAToZ", null),


                this.Url.Route("ConstituencyByID", new { id = "c6a51660-83a4-41eb-82c4-353b5b188caa" }),
                this.Url.Route("ConstituencyByInitial", new { initial = "l" }),
                this.Url.Route("ConstituencyCurrent", null),
                this.Url.Route("ConstituencyLookup", new { source = "onsCode", id = "E14000699" }),
                this.Url.Route("ConstituencyByLetters", new { letters = "heath" }),
                this.Url.Route("ConstituencyCurrentByInitial", new { initial = "v" }),
                this.Url.Route("ConstituencyAToZ", null),
                this.Url.Route("ConstituencyCurrentAToZ", null),


                this.Url.Route("PartyById", new { id = "ac265389-9e3d-4d3b-8d98-4d4b8b07bae4" }),
                this.Url.Route("PartyByInitial", new { initial = "a" }),
                this.Url.Route("PartyCurrent", null),
                this.Url.Route("PartyByLetters", new { letters = "zeb" }),
                this.Url.Route("PartyAToZ", null),
                this.Url.Route("PartyCurrentAToZ", null),
                this.Url.Route("PartyLookup", new { source = "mnisId", id = "231" }),


                this.Url.Route("HouseById", new { id = "4b77dd58-f6ba-4121-b521-c8ad70465f52" }),
                this.Url.Route("HouseLookup", new { source = "name", id = "House of Lords" }),
                this.Url.Route("HouseByLetters", new { letters = "house" }),

                this.Url.Route("ContactPointIndex", null),
                this.Url.Route("ContactPointById", new { id = "71cb6459-363d-4278-b6f1-04c497be95a1" }),


            };

            var response = Request.CreateResponse();

            response.Content = new StringContent($@"
<!DOCTYPE html>
<html>
    <body>
        <ul>{string.Join(string.Empty, from link in links select $"<li><a href='{link}'>{HttpUtility.UrlDecode(link).Substring(this.Configuration.VirtualPathRoot.Length)}</a>")}</ul>
    </body>
</html>
");

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}