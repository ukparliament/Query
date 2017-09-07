namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;
    using System.Web.Http;

    public partial class HelpController : BaseController
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var links = new string[] {
                this.Url.Route("WithoutExtension", new {name = "person_index" }),
                this.Url.Route("WithoutExtension", new {name = "person_by_id", person_id = "toes2sa2" }),
                this.Url.Route("WithoutExtension", new {name = "person_by_initial", initial = "ö" }),
                this.Url.Route("WithoutExtension", new {name = "person_lookup", property = "mnisId", value = "3299" }),
                this.Url.Route("WithoutExtension", new {name = "person_by_substring", substring = "ee" }),
                this.Url.Route("WithoutExtension", new {name = "person_a_to_z" }),
                this.Url.Route("WithoutExtension", new {name = "person_constituencies", person_id = "7KNGxTli" }),
                this.Url.Route("WithoutExtension", new {name = "person_current_constituency", person_id = "7KNGxTli" }),
                this.Url.Route("WithoutExtension", new {name = "person_parties", person_id = "7KNGxTli" }),
                this.Url.Route("WithoutExtension", new {name = "person_current_party", person_id = "7KNGxTli" }),
                this.Url.Route("WithoutExtension", new {name = "person_contact_points", person_id = "7KNGxTli" }),
                this.Url.Route("WithoutExtension", new {name = "person_houses", person_id = "7KNGxTli" }),
                this.Url.Route("WithoutExtension", new {name = "person_current_house", person_id = "7KNGxTli" }),
                this.Url.Route("WithoutExtension", new {name = "person_mps" }),

                // MemberIndex route exists on person controller
                this.Url.Route("WithoutExtension", new {name = "member_index" }),

                this.Url.Route("WithoutExtension", new {name = "member_by_initial", initial = "y" }),
                this.Url.Route("WithoutExtension", new {name = "member_current" }),
                this.Url.Route("WithoutExtension", new {name = "member_current_by_initial", initial = "z" }),
                this.Url.Route("WithoutExtension", new {name = "member_a_to_z" }),
                this.Url.Route("WithoutExtension", new {name = "member_current_a_to_z" }),

                this.Url.Route("WithoutExtension", new {name = "constituency_index" }),
                this.Url.Route("WithoutExtension", new {name = "constituency_by_id", constituency_id = "dwtSdieB" }),
                this.Url.Route("WithoutExtension", new {name = "constituency_map", constituency_id = "dwtSdieB" }),
                this.Url.Route("WithoutExtension", new {name = "constituency_by_initial", initial = "l" }),
                this.Url.Route("WithoutExtension", new {name = "constituency_current" }),
                this.Url.Route("WithoutExtension", new {name = "constituency_lookup", property = "onsCode", value = "E14000699" }),
                this.Url.Route("WithoutExtension", new {name = "constituency_by_substring", substring = "heath" }),
                this.Url.Route("WithoutExtension", new {name = "constituency_current_by_initial", initial = "v" }),
                this.Url.Route("WithoutExtension", new {name = "constituency_a_to_z"}),
                this.Url.Route("WithoutExtension", new {name = "constituency_current_a_to_z"}),
                this.Url.Route("WithoutExtension", new {name = "constituency_members", constituency_id = "dwtSdieB" }),
                this.Url.Route("WithoutExtension", new {name = "constituency_current_member", constituency_id = "dwtSdieB" }),
                this.Url.Route("WithoutExtension", new {name = "constituency_contact_point", constituency_id = "dwtSdieB" }),
                this.Url.Route("WithoutExtension", new {name = "constituency_lookup_by_postcode",postcode = "sw1p 3ja"}),
                this.Url.Route("WithoutExtension", new {name = "find_your_constituency" }),

                this.Url.Route("WithoutExtension", new {name = "party_index"}),
                this.Url.Route("WithoutExtension", new {name = "party_by_id", party_id = "891w1b1k" }),
                this.Url.Route("WithoutExtension", new {name = "party_by_initial", initial = "a" }),
                this.Url.Route("WithoutExtension", new {name = "party_current"}),
                this.Url.Route("WithoutExtension", new {name = "party_by_substring", substring = "lock" }),
                this.Url.Route("WithoutExtension", new {name = "party_a_to_z"}),
                this.Url.Route("WithoutExtension", new {name = "party_current_a_to_z"}),
                this.Url.Route("WithoutExtension", new {name = "party_lookup", property = "mnisId", value = "231" }),
                this.Url.Route("WithoutExtension", new {name = "party_members", party_id = "891w1b1k"}),
                this.Url.Route("WithoutExtension", new {name = "party_current_members", party_id = "891w1b1k"}),
                this.Url.Route("WithoutExtension", new {name = "party_members_by_initial", party_id = "891w1b1k", initial = "f"}),
                this.Url.Route("WithoutExtension", new {name = "party_members_a_to_z", party_id = "891w1b1k" }),
                this.Url.Route("WithoutExtension", new {name = "party_current_members_by_initial", party_id = "891w1b1k", initial = "z"}),
                this.Url.Route("WithoutExtension", new {name = "party_current_members_a_to_z", party_id = "891w1b1k" }),

                this.Url.Route("WithoutExtension", new {name = "house_index"}),
                this.Url.Route("WithoutExtension", new {name = "house_by_id", house_id = "Kz7ncmrt" }),
                this.Url.Route("WithoutExtension", new {name = "house_lookup", property = "name", value = "House of Lords" }),
                this.Url.Route("WithoutExtension", new {name = "house_by_substring", substring = "house" }),
                this.Url.Route("WithoutExtension", new {name = "house_members", house_id = "Kz7ncmrt"}),
                this.Url.Route("WithoutExtension", new {name = "house_current_members", house_id = "Kz7ncmrt"}),
                this.Url.Route("WithoutExtension", new {name = "house_parties", house_id = "Kz7ncmrt"}),
                this.Url.Route("WithoutExtension", new {name = "house_current_parties", house_id = "Kz7ncmrt"}),
                this.Url.Route("WithoutExtension", new {name = "house_party_by_id", house_id = "Kz7ncmrt", party_id = "891w1b1k"}),
                this.Url.Route("WithoutExtension", new {name = "house_members_by_initial", house_id = "Kz7ncmrt", initial = "m"}),
                this.Url.Route("WithoutExtension", new {name = "house_members_a_to_z", house_id = "Kz7ncmrt" }),
                this.Url.Route("WithoutExtension", new {name = "house_current_members_by_initial", house_id = "Kz7ncmrt", initial = "m"}),
                this.Url.Route("WithoutExtension", new {name = "house_current_members_a_to_z", house_id = "Kz7ncmrt" }),
                this.Url.Route("WithoutExtension", new {name = "house_party_members", house_id = "Kz7ncmrt", party_id = "891w1b1k"}),
                this.Url.Route("WithoutExtension", new {name = "house_party_members_by_initial", house_id = "Kz7ncmrt", party_id = "891w1b1k", initial = "c"}),
                this.Url.Route("WithoutExtension", new {name = "house_party_members_a_to_z", house_id = "Kz7ncmrt", party_id = "891w1b1k"}),
                this.Url.Route("WithoutExtension", new {name = "house_party_current_members", house_id = "Kz7ncmrt", party_id = "891w1b1k"}),
                this.Url.Route("WithoutExtension", new {name = "house_party_current_members_by_initial", house_id = "Kz7ncmrt", party_id = "891w1b1k", initial = "f"}),
                this.Url.Route("WithoutExtension", new {name = "house_party_current_members_a_to_z", house_id = "Kz7ncmrt", party_id = "891w1b1k"}),

                this.Url.Route("WithoutExtension", new {name = "contact_point_index"}),
                this.Url.Route("WithoutExtension", new {name = "contact_point_by_id", contact_point_id = "t3Qeaou5" }),

                this.Url.Route("WithoutExtension", new {name = "parliament_index"}),
                this.Url.Route("WithoutExtension", new {name = "parliament_current"}),
                this.Url.Route("WithoutExtension", new {name = "parliament_next"}),
                this.Url.Route("WithoutExtension", new {name = "parliament_previous"}),
                this.Url.Route("WithoutExtension", new {name = "parliament_lookup", property = "parliamentPeriodNumber", value = "56" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_by_id", parliament_id = "fHx6P1lb" }),
                this.Url.Route("WithoutExtension", new {name = "next_parliament_by_id", parliament_id = "fHx6P1lb" }),
                this.Url.Route("WithoutExtension", new {name = "previous_parliament_by_id", parliament_id = "fHx6P1lb" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_members", parliament_id = "fHx6P1lb" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_members_by_initial", parliament_id = "fHx6P1lb", initial = "d" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_members_a_to_z", parliament_id = "fHx6P1lb" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_houses", parliament_id = "fHx6P1lb" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_house", parliament_id = "fHx6P1lb", house_id = "Kz7ncmrt" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_house_members", parliament_id = "fHx6P1lb", house_id = "Kz7ncmrt" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_house_members_a_to_z", parliament_id = "fHx6P1lb", house_id = "Kz7ncmrt" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_house_members_by_initial", parliament_id = "fHx6P1lb", house_id = "Kz7ncmrt", initial = "d" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_parties", parliament_id = "fHx6P1lb" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_party", parliament_id = "fHx6P1lb", party_id = "891w1b1k" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_party_members", parliament_id = "fHx6P1lb", party_id = "891w1b1k" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_party_members_a_to_z", parliament_id = "fHx6P1lb", party_id = "891w1b1k" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_party_members_by_initial", parliament_id = "fHx6P1lb", party_id = "891w1b1k", initial = "d" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_house_parties", parliament_id = "fHx6P1lb", house_id = "Kz7ncmrt" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_house_party", parliament_id = "fHx6P1lb", house_id = "Kz7ncmrt", party_id = "891w1b1k"}),
                this.Url.Route("WithoutExtension", new {name = "parliament_house_party_members", parliament_id = "fHx6P1lb", house_id = "Kz7ncmrt", party_id = "891w1b1k"}),
                this.Url.Route("WithoutExtension", new {name = "parliament_house_party_members_a_to_z", parliament_id = "fHx6P1lb", house_id = "Kz7ncmrt", party_id = "891w1b1k"}),
                this.Url.Route("WithoutExtension", new {name = "parliament_house_party_members_by_initial", parliament_id = "fHx6P1lb", house_id = "Kz7ncmrt", party_id = "891w1b1k", initial = "d"}),
                this.Url.Route("WithoutExtension", new {name = "parliament_constituencies", parliament_id = "fHx6P1lb" }),
                this.Url.Route("WithoutExtension", new {name = "parliament_constituencies_a_to_z", parliament_id = "fHx6P1lb"}),
                this.Url.Route("WithoutExtension", new {name = "parliament_constituencies_by_initial", parliament_id = "fHx6P1lb", initial = "d" }),

                this.Url.Route("WithoutExtension", new {name = "image_by_id", image_id = "qnsCGpnw"}),

                this.Url.Route("WithoutExtension", new {name = "region_index"}),
                this.Url.Route("WithoutExtension", new {name = "region_by_id", region_code = "E15000001" }),
                this.Url.Route("WithoutExtension", new {name = "region_constituencies", region_code = "E15000001" }),
                this.Url.Route("WithoutExtension", new {name = "region_constituencies_a_to_z", region_code = "E15000001" }),
                this.Url.Route("WithoutExtension", new {name = "region_constituencies_by_initial", region_code = "E15000001", initial = "h" }),

                this.Url.Route("WithoutExtension", new {name = "formal_body_index" }),
                this.Url.Route("WithoutExtension", new {name = "formal_body_by_id", formal_body_id="S2k3z9ww" }),
                this.Url.Route("WithoutExtension", new {name = "formal_body_membership", formal_body_id="S2k3z9ww" })
            } as IEnumerable<string>;

            // Make links relative, remove application virtual path (in this case, a trailing forward slash).
            links = from link in links select HttpUtility.UrlDecode(link).Substring(this.Configuration.VirtualPathRoot.Length);

            var response = Request.CreateResponse();

            response.Content = new StringContent($@"
<!DOCTYPE html>
<html>
    <head>
        <meta charset='utf-8'>
    </head>
    <body>
        <ul>{string.Join(string.Empty, from link in links select $"<li><a href='{link}'>{link}</a></li>")}</ul>
    </body>
</html>
");

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}
