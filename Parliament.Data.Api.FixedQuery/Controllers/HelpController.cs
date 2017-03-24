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


                // MemberIndex route exists on person controller
                this.Url.Route("MemberIndex", null),

                this.Url.Route("MemberCurrent", null),
                this.Url.Route("MemberByInitial", new { initial = "y" }),
                this.Url.Route("MemberCurrentByInitial", new { initial = "z" }),
                this.Url.Route("MemberAToZ", null),
                this.Url.Route("MemberCurrentAToZ", null),

                
                this.Url.Route("ConstituencyIndex", null),
                this.Url.Route("ConstituencyByID", new { id = "c6a51660-83a4-41eb-82c4-353b5b188caa" }),
                this.Url.Route("ConstituencyByInitial", new { initial = "l" }),
                this.Url.Route("ConstituencyCurrent", null),
                this.Url.Route("ConstituencyLookup", new { source = "onsCode", id = "E14000699" }),
                this.Url.Route("ConstituencyByLetters", new { letters = "heath" }),
                this.Url.Route("ConstituencyCurrentByInitial", new { initial = "v" }),
                this.Url.Route("ConstituencyAToZ", null),
                this.Url.Route("ConstituencyCurrentAToZ", null),
                this.Url.Route("ConstituencyMembers", new { id = "0dc99b0c-63a1-4c47-949b-cf836d557708" }),
                this.Url.Route("ConstituencyCurrentMember", new { id = "0dc99b0c-63a1-4c47-949b-cf836d557708" }),
                this.Url.Route("ConstituencyContactPoint", new { id = "0dc99b0c-63a1-4c47-949b-cf836d557708" }),


                this.Url.Route("PartyIndex", null),
                this.Url.Route("PartyById", new { id = "ac265389-9e3d-4d3b-8d98-4d4b8b07bae4" }),
                this.Url.Route("PartyByInitial", new { initial = "a" }),
                this.Url.Route("PartyCurrent", null),
                this.Url.Route("PartyByLetters", new { letters = "zeb" }),
                this.Url.Route("PartyAToZ", null),
                this.Url.Route("PartyCurrentAToZ", null),
                this.Url.Route("PartyLookup", new { source = "mnisId", id = "231" }),
                this.Url.Route("PartyMembers", new { id = "c5858995-6d25-4eb5-b92e-fba3fbd8ba47"}),
                this.Url.Route("PartyCurrentMembers", new { id = "c5858995-6d25-4eb5-b92e-fba3fbd8ba47"}),
                this.Url.Route("PartyMembersByInitial", new { id = "c5858995-6d25-4eb5-b92e-fba3fbd8ba47", initial = "f"}),
                this.Url.Route("PartyMembersAToZ", new { id = "c5858995-6d25-4eb5-b92e-fba3fbd8ba47" }),
                this.Url.Route("PartyCurrentMembersByInitial", new { id = "c5858995-6d25-4eb5-b92e-fba3fbd8ba47", initial = "z"}),
                this.Url.Route("PartyCurrentMembersAToZ", new { id = "c5858995-6d25-4eb5-b92e-fba3fbd8ba47" }),


                this.Url.Route("HouseIndex", null),
                this.Url.Route("HouseById", new { id = "4b77dd58-f6ba-4121-b521-c8ad70465f52" }),
                this.Url.Route("HouseLookup", new { source = "name", id = "House of Lords" }),
                this.Url.Route("HouseByLetters", new { letters = "house" }),
                this.Url.Route("HouseMembers", new { id = "4b77dd58-f6ba-4121-b521-c8ad70465f52"}),
                this.Url.Route("HouseCurrentMembers", new { id = "4b77dd58-f6ba-4121-b521-c8ad70465f52"}),
                this.Url.Route("HouseParties", new { id = "4b77dd58-f6ba-4121-b521-c8ad70465f52"}),
                this.Url.Route("HouseCurrentParties", new { id = "4b77dd58-f6ba-4121-b521-c8ad70465f52"}),
                this.Url.Route("HousePartyById", new { houseid = "4b77dd58-f6ba-4121-b521-c8ad70465f52", partyid = "61080972-c215-42a9-a04a-10e0002e1c18"}),
                this.Url.Route("HouseMembersByInitial", new { houseid = "4b77dd58-f6ba-4121-b521-c8ad70465f52", initial = "m"}),
                this.Url.Route("HouseMembersAToZ", new { id = "4b77dd58-f6ba-4121-b521-c8ad70465f52" }),
                this.Url.Route("HouseCurrentMembersByInitial", new { houseid = "4b77dd58-f6ba-4121-b521-c8ad70465f52", initial = "m"}),
                this.Url.Route("HouseCurrentMembersAToZ", new { id = "4b77dd58-f6ba-4121-b521-c8ad70465f52" }),
                this.Url.Route("HousePartyMembers", new { houseid = "4b77dd58-f6ba-4121-b521-c8ad70465f52", partyid = "61080972-c215-42a9-a04a-10e0002e1c18"}),
                this.Url.Route("HousePartyMembersByInitial", new { houseid = "4b77dd58-f6ba-4121-b521-c8ad70465f52", partyid = "61080972-c215-42a9-a04a-10e0002e1c18", initial = "c"}),
                this.Url.Route("HousePartyMembersAToZ", new { houseid = "4b77dd58-f6ba-4121-b521-c8ad70465f52", partyid = "61080972-c215-42a9-a04a-10e0002e1c18"}),
                this.Url.Route("HousePartyCurrentMembers", new { houseid = "4b77dd58-f6ba-4121-b521-c8ad70465f52", partyid = "61080972-c215-42a9-a04a-10e0002e1c18"}),
                this.Url.Route("HousePartyCurrentMembersByInitial", new { houseid = "4b77dd58-f6ba-4121-b521-c8ad70465f52", partyid = "61080972-c215-42a9-a04a-10e0002e1c18", initial = "f"}),
                this.Url.Route("HousePartyCurrentMembersAToZ", new { houseid = "4b77dd58-f6ba-4121-b521-c8ad70465f52", partyid = "61080972-c215-42a9-a04a-10e0002e1c18"}),


                this.Url.Route("ContactPointIndex", null),
                this.Url.Route("ContactPointById", new { id = "71cb6459-363d-4278-b6f1-04c497be95a1" })


            };

            var response = Request.CreateResponse();

            response.Content = new StringContent($@"
<!DOCTYPE html>
<html>
    <head>
        <meta charset='utf-8'>
    </head>
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