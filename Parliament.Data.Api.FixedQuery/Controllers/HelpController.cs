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

    public partial class HelpController : BaseController 
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var links = new string[] {
                this.Url.Route("WithoutExtension", new {action = "PersonIndex" }),
                this.Url.Route("WithoutExtension", new {action = "PersonById", id = "Cz0WNho9" }),
                this.Url.Route("WithoutExtension", new {action = "PersonByInitial", initial = "ö" }),
                this.Url.Route("WithoutExtension", new {action = "PersonLookup", source = "mnisId", id = "3299" }),
                this.Url.Route("WithoutExtension", new {action = "PersonByLetters", letters = "ee" }),
                this.Url.Route("WithoutExtension", new {action = "PersonAToZ" }),
                this.Url.Route("WithoutExtension", new {action = "PersonConstituencies", id = "Cz0WNho9" }),
                this.Url.Route("WithoutExtension", new {action = "PersonCurrentConstituency", id = "Cz0WNho9" }),
                this.Url.Route("WithoutExtension", new {action = "PersonParties", id = "Cz0WNho9" }),
                this.Url.Route("WithoutExtension", new {action = "PersonCurrentParty", id = "Cz0WNho9" }),
                this.Url.Route("WithoutExtension", new {action = "PersonContactPoints", id = "Cz0WNho9" }),
                this.Url.Route("WithoutExtension", new {action = "PersonHouses", id = "Cz0WNho9" }),
                this.Url.Route("WithoutExtension", new {action = "PersonCurrentHouse", id = "Cz0WNho9" }),
                this.Url.Route("WithoutExtension", new {action = "PersonMPs" }),

                // MemberIndex route exists on person controller
                this.Url.Route("WithoutExtension", new {action = "MemberIndex" }),

                this.Url.Route("WithoutExtension", new {action = "MemberByInitial", initial = "y" }),
                this.Url.Route("WithoutExtension", new {action = "MemberCurrent" }),
                this.Url.Route("WithoutExtension", new {action = "MemberCurrentByInitial", initial = "z" }),
                this.Url.Route("WithoutExtension", new {action = "MemberAToZ" }),
                this.Url.Route("WithoutExtension", new {action = "MemberCurrentAToZ" }),

                this.Url.Route("WithoutExtension", new {action = "ConstituencyIndex" }),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyByID", id = "tiIAowt8" }),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyMap", id = "tiIAowt8" }),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyByInitial", initial = "l" }),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyCurrent" }),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyLookup", source = "onsCode", id = "E14000699" }),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyByLetters", letters = "heath" }),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyCurrentByInitial", initial = "v" }),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyAToZ"}),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyCurrentAToZ"}),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyMembers", id = "EiwoTrI2" }),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyCurrentMember", id = "EiwoTrI2" }),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyContactPoint", id = "EiwoTrI2" }),
                this.Url.Route("WithoutExtension", new {action = "ConstituencyLookupByPostcode",postcode = "sw1p 3ja"}),

                this.Url.Route("WithoutExtension", new {action = "PartyIndex"}),
                this.Url.Route("WithoutExtension", new {action = "PartyById", id = "P6LNyUn4" }),
                this.Url.Route("WithoutExtension", new {action = "PartyByInitial", initial = "a" }),
                this.Url.Route("WithoutExtension", new {action = "PartyCurrent"}),
                this.Url.Route("WithoutExtension", new {action = "PartyByLetters", letters = "zeb" }),
                this.Url.Route("WithoutExtension", new {action = "PartyAToZ"}),
                this.Url.Route("WithoutExtension", new {action = "PartyCurrentAToZ"}),
                this.Url.Route("WithoutExtension", new {action = "PartyLookup", source = "mnisId", id = "231" }),
                this.Url.Route("WithoutExtension", new {action = "PartyMembers", id = "lk3RZ8EB"}),
                this.Url.Route("WithoutExtension", new {action = "PartyCurrentMembers", id = "lk3RZ8EB"}),
                this.Url.Route("WithoutExtension", new {action = "PartyMembersByInitial", id = "lk3RZ8EB", initial = "f"}),
                this.Url.Route("WithoutExtension", new {action = "PartyMembersAToZ", id = "lk3RZ8EB" }),
                this.Url.Route("WithoutExtension", new {action = "PartyCurrentMembersByInitial", id = "lk3RZ8EB", initial = "z"}),
                this.Url.Route("WithoutExtension", new {action = "PartyCurrentMembersAToZ", id = "lk3RZ8EB" }),

                this.Url.Route("WithoutExtension", new {action = "HouseIndex"}),
                this.Url.Route("WithoutExtension", new {action = "HouseById", id = "KL2k1BGP" }),
                this.Url.Route("WithoutExtension", new {action = "HouseLookup", source = "name", id = "House of Lords" }),
                this.Url.Route("WithoutExtension", new {action = "HouseByLetters", letters = "house" }),
                this.Url.Route("WithoutExtension", new {action = "HouseMembers", id = "KL2k1BGP"}),
                this.Url.Route("WithoutExtension", new {action = "HouseCurrentMembers", id = "KL2k1BGP"}),
                this.Url.Route("WithoutExtension", new {action = "HouseParties", id = "KL2k1BGP"}),
                this.Url.Route("WithoutExtension", new {action = "HouseCurrentParties", id = "KL2k1BGP"}),
                this.Url.Route("WithoutExtension", new {action = "HousePartyById", houseid = "KL2k1BGP", partyid = "P6LNyUn4"}),
                this.Url.Route("WithoutExtension", new {action = "HouseMembersByInitial", houseid = "KL2k1BGP", initial = "m"}),
                this.Url.Route("WithoutExtension", new {action = "HouseMembersAToZ", id = "KL2k1BGP" }),
                this.Url.Route("WithoutExtension", new {action = "HouseCurrentMembersByInitial", houseid = "KL2k1BGP", initial = "m"}),
                this.Url.Route("WithoutExtension", new {action = "HouseCurrentMembersAToZ", id = "KL2k1BGP" }),
                this.Url.Route("WithoutExtension", new {action = "HousePartyMembers", houseid = "KL2k1BGP", partyid = "P6LNyUn4"}),
                this.Url.Route("WithoutExtension", new {action = "HousePartyMembersByInitial", houseid = "KL2k1BGP", partyid = "jMNf7IDk", initial = "c"}),
                this.Url.Route("WithoutExtension", new {action = "HousePartyMembersAToZ", houseid = "KL2k1BGP", partyid = "P6LNyUn4"}),
                this.Url.Route("WithoutExtension", new {action = "HousePartyCurrentMembers", houseid = "KL2k1BGP", partyid = "P6LNyUn4"}),
                this.Url.Route("WithoutExtension", new {action = "HousePartyCurrentMembersByInitial", houseid = "KL2k1BGP", partyid = "P6LNyUn4", initial = "f"}),
                this.Url.Route("WithoutExtension", new {action = "HousePartyCurrentMembersAToZ", houseid = "KL2k1BGP", partyid = "P6LNyUn4"}),

                this.Url.Route("WithoutExtension", new {action = "ContactPointIndex"}),
                this.Url.Route("WithoutExtension", new {action = "ContactPointById", id = "Ne6xySIb" }),

                this.Url.Route("WithoutExtension", new {action = "ParliamentIndex"}),
                this.Url.Route("WithoutExtension", new {action = "ParliamentCurrent"}),
                this.Url.Route("WithoutExtension", new {action = "ParliamentNext"}),
                this.Url.Route("WithoutExtension", new {action = "ParliamentPrevious"}),
                this.Url.Route("WithoutExtension", new {action = "ParliamentLookup", source = "parliamentPeriodNumber", id = "56" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentById", id = "Du6iH79e" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentNext", id = "Du6iH79e" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentPrevious", id = "Du6iH79e" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentMembers", id = "Du6iH79e" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentMembersByInitial", id = "Du6iH79e", initial = "d" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentMembersAToZLetters", id = "Du6iH79e" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentHouses", id = "Du6iH79e" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentHouse", parliamentid = "Du6iH79e", houseid = "cqIATgUK" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentHouseMembers", parliamentid = "Du6iH79e", houseid = "cqIATgUK" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentHouseMembersAToZLetters", parliamentid = "Du6iH79e", houseid = "cqIATgUK" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentHouseMembersByInitial", parliamentid = "Du6iH79e", houseid = "cqIATgUK", initial = "d" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentParties", id = "Du6iH79e" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentParty", parliamentid = "Du6iH79e", partyid = "6THXr8R6" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentPartyMembers", parliamentid = "Du6iH79e", partyid = "6THXr8R6" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentPartyMembersAToZLetters", parliamentid = "Du6iH79e", partyid = "6THXr8R6" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentPartyMembersByInitial", parliamentid = "Du6iH79e", partyid = "6THXr8R6", initial = "d" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentHouseParties", parliamentid = "Du6iH79e", houseid = "cqIATgUK" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentHouseParty", parliamentid = "Du6iH79e", houseid = "cqIATgUK", partyid = "6THXr8R6"}),
                this.Url.Route("WithoutExtension", new {action = "ParliamentHousePartyMembers", parliamentid = "Du6iH79e", houseid = "cqIATgUK", partyid = "6THXr8R6"}),
                this.Url.Route("WithoutExtension", new {action = "ParliamentHousePartyMembersAToZLetters", parliamentid = "Du6iH79e", houseid = "cqIATgUK", partyid = "6THXr8R6"}),
                this.Url.Route("WithoutExtension", new {action = "ParliamentHousePartyMembersByInitial", parliamentid = "Du6iH79e", houseid = "cqIATgUK", partyid = "6THXr8R6", initial = "d"}),
                this.Url.Route("WithoutExtension", new {action = "ParliamentConstituencies", id = "Du6iH79e" }),
                this.Url.Route("WithoutExtension", new {action = "ParliamentConstituenciesAToZLetters", id = "Du6iH79e"}),
                this.Url.Route("WithoutExtension", new {action = "ParliamentConstituenciesByInitial", id = "0FxbTVtr", initial = "d" })
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
    }
}
