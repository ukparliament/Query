namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;

    [RoutePrefix("")]
    public class IndexController : BaseController
    {
        [Route()]
        public HttpResponseMessage Get()
        {
            var links = new string[] {
                this.Url.Route("PersonIndex", null),
                this.Url.Route("PersonById", new { id = "Cz0WNho9" }),
                this.Url.Route("PersonByInitial", new { initial = "ö" }),
                this.Url.Route("PersonLookup", new { source = "mnisId", id = "3299" }),
                this.Url.Route("PersonByLetters", new { letters = "ee" }),
                this.Url.Route("PersonAToZ", null),
                this.Url.Route("PersonConstituencies", new { id = "Cz0WNho9" }),
                this.Url.Route("PersonCurrentConstituency", new { id = "Cz0WNho9" }),
                this.Url.Route("PersonParties", new { id = "Cz0WNho9" }),
                this.Url.Route("PersonCurrentParty", new { id = "Cz0WNho9" }),
                this.Url.Route("PersonContactPoints", new { id = "Cz0WNho9" }),
                this.Url.Route("PersonHouses", new { id = "Cz0WNho9" }),
                this.Url.Route("PersonCurrentHouse", new { id = "Cz0WNho9" }),

                // MemberIndex route exists on person controller
                this.Url.Route("MemberIndex", null),

                this.Url.Route("MemberCurrent", null),
                this.Url.Route("MemberByInitial", new { initial = "y" }),
                this.Url.Route("MemberCurrentByInitial", new { initial = "z" }),
                this.Url.Route("MemberAToZ", null),
                this.Url.Route("MemberCurrentAToZ", null),

                this.Url.Route("ConstituencyIndex", null),
                this.Url.Route("ConstituencyByID", new { id = "EiwoTrI2" }),
                this.Url.Route("ConstituencyByInitial", new { initial = "l" }),
                this.Url.Route("ConstituencyCurrent", null),
                this.Url.Route("ConstituencyLookup", new { source = "onsCode", id = "E14000699" }),
                this.Url.Route("ConstituencyByLetters", new { letters = "heath" }),
                this.Url.Route("ConstituencyCurrentByInitial", new { initial = "v" }),
                this.Url.Route("ConstituencyAToZ", null),
                this.Url.Route("ConstituencyCurrentAToZ", null),
                this.Url.Route("ConstituencyMembers", new { id = "EiwoTrI2" }),
                this.Url.Route("ConstituencyCurrentMember", new { id = "EiwoTrI2" }),
                this.Url.Route("ConstituencyContactPoint", new { id = "EiwoTrI2" }),
                this.Url.Route("ConstituencyLookupByPostcode", new { postcode = "sw1p 3ja"}),

                this.Url.Route("PartyIndex", null),
                this.Url.Route("PartyById", new { id = "P6LNyUn4" }),
                this.Url.Route("PartyByInitial", new { initial = "a" }),
                this.Url.Route("PartyCurrent", null),
                this.Url.Route("PartyByLetters", new { letters = "zeb" }),
                this.Url.Route("PartyAToZ", null),
                this.Url.Route("PartyCurrentAToZ", null),
                this.Url.Route("PartyLookup", new { source = "mnisId", id = "231" }),
                this.Url.Route("PartyMembers", new { id = "lk3RZ8EB"}),
                this.Url.Route("PartyCurrentMembers", new { id = "lk3RZ8EB"}),
                this.Url.Route("PartyMembersByInitial", new { id = "lk3RZ8EB", initial = "f"}),
                this.Url.Route("PartyMembersAToZ", new { id = "lk3RZ8EB" }),
                this.Url.Route("PartyCurrentMembersByInitial", new { id = "lk3RZ8EB", initial = "z"}),
                this.Url.Route("PartyCurrentMembersAToZ", new { id = "lk3RZ8EB" }),

                this.Url.Route("HouseIndex", null),
                this.Url.Route("HouseById", new { id = "KL2k1BGP" }),
                this.Url.Route("HouseLookup", new { source = "name", id = "House of Lords" }),
                this.Url.Route("HouseByLetters", new { letters = "house" }),
                this.Url.Route("HouseMembers", new { id = "KL2k1BGP"}),
                this.Url.Route("HouseCurrentMembers", new { id = "KL2k1BGP"}),
                this.Url.Route("HouseParties", new { id = "KL2k1BGP"}),
                this.Url.Route("HouseCurrentParties", new { id = "KL2k1BGP"}),
                this.Url.Route("HousePartyById", new { houseid = "KL2k1BGP", partyid = "P6LNyUn4"}),
                this.Url.Route("HouseMembersByInitial", new { houseid = "KL2k1BGP", initial = "m"}),
                this.Url.Route("HouseMembersAToZ", new { id = "KL2k1BGP" }),
                this.Url.Route("HouseCurrentMembersByInitial", new { houseid = "KL2k1BGP", initial = "m"}),
                this.Url.Route("HouseCurrentMembersAToZ", new { id = "KL2k1BGP" }),
                this.Url.Route("HousePartyMembers", new { houseid = "KL2k1BGP", partyid = "P6LNyUn4"}),
                this.Url.Route("HousePartyMembersByInitial", new { houseid = "KL2k1BGP", partyid = "jMNf7IDk", initial = "c"}),
                this.Url.Route("HousePartyMembersAToZ", new { houseid = "KL2k1BGP", partyid = "P6LNyUn4"}),
                this.Url.Route("HousePartyCurrentMembers", new { houseid = "KL2k1BGP", partyid = "P6LNyUn4"}),
                this.Url.Route("HousePartyCurrentMembersByInitial", new { houseid = "KL2k1BGP", partyid = "P6LNyUn4", initial = "f"}),
                this.Url.Route("HousePartyCurrentMembersAToZ", new { houseid = "KL2k1BGP", partyid = "P6LNyUn4"}),

                this.Url.Route("ContactPointIndex", null),
                this.Url.Route("ContactPointById", new { id = "Ne6xySIb" }),

                this.Url.Route("ParliamentIndex", null),
                this.Url.Route("ParliamentCurrent", null),
                this.Url.Route("ParliamentNext", null),
                this.Url.Route("ParliamentPrevious", null),
                this.Url.Route("ParliamentLookup", new { source = "parliamentPeriodNumber", id = "56" }),
                this.Url.Route("ParliamentById", new { id = "Du6iH79e" }),
                this.Url.Route("ParliamentNext", new { id = "Du6iH79e" }),
                this.Url.Route("ParliamentPrevious", new { id = "Du6iH79e" }),
                this.Url.Route("ParliamentMembers", new { id = "Du6iH79e" }),
                this.Url.Route("ParliamentMembersByInitial", new { id = "Du6iH79e", initial = "d" }),
                this.Url.Route("ParliamentMembersAToZLetters", new { id = "Du6iH79e" }),
                this.Url.Route("ParliamentHouses", new { id = "Du6iH79e" }),
                this.Url.Route("ParliamentHouse", new { parliamentid = "Du6iH79e", houseid = "cqIATgUK" }),
                this.Url.Route("ParliamentHouseMembers", new { parliamentid = "Du6iH79e", houseid = "cqIATgUK" }),
                this.Url.Route("ParliamentHouseMembersAToZLetters", new { parliamentid = "Du6iH79e", houseid = "cqIATgUK" }),
                this.Url.Route("ParliamentHouseMembersByInitial", new { parliamentid = "Du6iH79e", houseid = "cqIATgUK", initial = "d" }),
                this.Url.Route("ParliamentParties", new { id = "Du6iH79e" }),
                this.Url.Route("ParliamentParty", new { parliamentid = "Du6iH79e", partyid = "6THXr8R6" }),
                this.Url.Route("ParliamentPartyMembers", new { parliamentid = "Du6iH79e", partyid = "6THXr8R6" }),
                this.Url.Route("ParliamentPartyMembersAToZLetters", new { parliamentid = "Du6iH79e", partyid = "6THXr8R6" }),
                this.Url.Route("ParliamentPartyMembersByInitial", new { parliamentid = "Du6iH79e", partyid = "6THXr8R6", initial = "d" }),
                this.Url.Route("ParliamentHouseParties", new { parliamentid = "Du6iH79e", houseid = "cqIATgUK" }),
                this.Url.Route("ParliamentHouseParty", new { parliamentid = "Du6iH79e", houseid = "cqIATgUK", partyid = "6THXr8R6"}),
                this.Url.Route("ParliamentHousePartyMembers", new { parliamentid = "Du6iH79e", houseid = "cqIATgUK", partyid = "6THXr8R6"}),
                this.Url.Route("ParliamentHousePartyMembersAToZLetters", new { parliamentid = "Du6iH79e", houseid = "cqIATgUK", partyid = "6THXr8R6"}),
                this.Url.Route("ParliamentHousePartyMembersByInitial", new { parliamentid = "Du6iH79e", houseid = "cqIATgUK", partyid = "6THXr8R6", initial = "d"}),
                this.Url.Route("ParliamentConstituencies", new { id = "Du6iH79e" }),
                this.Url.Route("ParliamentConstituenciesAToZLetters", new { id = "Du6iH79e"}),
                this.Url.Route("ParliamentConstituenciesByInitial", new { id = "0FxbTVtr", initial = "d" })
            };

            var response = Request.CreateResponse();

            response.Content = new StringContent($@"
<!DOCTYPE html>
<html>
    <head>
        <meta charset='utf-8'>
    </head>
    <body>
        <ul>{string.Join(string.Empty, from link in links select $"<li><a href='{link}'>{HttpUtility.UrlDecode(link).Substring(this.Configuration.VirtualPathRoot.Length)}</a></li>")}</ul>
    </body>
</html>
");

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }

        [Route(@"lookup/{type:regex(^\w+$)}/{source:regex(^\w+$)}/{id}", Name = "Lookup")]
        [HttpGet]
        public Graph Lookup(string type, string source, string id) => base.LookupInternal(type, source, id);
    }
}