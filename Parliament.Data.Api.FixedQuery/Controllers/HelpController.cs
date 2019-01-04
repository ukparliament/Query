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
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;
    using System.Web.Http.Description;

    public partial class HelpController : BaseController
    {
        public HttpResponseMessage Get()
        {
            var helpIds = new Dictionary<string, string>()
            {
                { "House Of Commons", "1AFu55Hs" },
                { "Ken Clarke", "TyNGhslR" },
                { "Vale of Glamorgan", "AEyWGYaP" },
                { "Labour", "LEYIBvV9" },
                { "Yeovil Lib Dems Contact", "wk1atnfh" },
                { "56th Parliament", "b0t56VVL"},
                { "Treasury Committee", "cLjFRjRt"},
                { "How to submit a written question online","8MHJ9zSp"},
                { "Oral questions", "yDYJSViV"}
            };

            var links = new string[] {
                this.Url.Route("WithoutExtension", new { name = "person_index" }),
                this.Url.Route("WithoutExtension", new { name = "person_by_id", person_id = helpIds["Ken Clarke"] }),
                this.Url.Route("WithoutExtension", new { name = "person_by_initial", initial = "ö" }),
                this.Url.Route("WithoutExtension", new { name = "person_lookup", property = "mnisId", value = "3299" }),
                this.Url.Route("WithoutExtension", new { name = "person_by_substring", substring = "ee" }),
                this.Url.Route("WithoutExtension", new { name = "person_a_to_z" }),
                this.Url.Route("WithoutExtension", new { name = "person_constituencies", person_id = helpIds["Ken Clarke"] }),
                this.Url.Route("WithoutExtension", new { name = "person_current_constituency", person_id = helpIds["Ken Clarke"] }),
                this.Url.Route("WithoutExtension", new { name = "person_parties", person_id = helpIds["Ken Clarke"] }),
                this.Url.Route("WithoutExtension", new { name = "person_current_party", person_id = helpIds["Ken Clarke"] }),
                this.Url.Route("WithoutExtension", new { name = "person_contact_points", person_id = helpIds["Ken Clarke"] }),
                this.Url.Route("WithoutExtension", new { name = "person_houses", person_id = helpIds["Ken Clarke"] }),
                this.Url.Route("WithoutExtension", new { name = "person_current_house", person_id = helpIds["Ken Clarke"] }),
                this.Url.Route("WithoutExtension", new { name = "person_mps" }),
                this.Url.Route("WithoutExtension", new { name = "person_committees_index", person_id = helpIds["Ken Clarke"] }),
                this.Url.Route("WithoutExtension", new { name = "person_committees_memberships_index", person_id = helpIds["Ken Clarke"] }),
                this.Url.Route("WithoutExtension", new { name = "person_current_committees_memberships", person_id = helpIds["Ken Clarke"] }),

                // MemberIndex route exists on person controller
                this.Url.Route("WithoutExtension", new { name = "member_index" }),

                this.Url.Route("WithoutExtension", new { name = "member_by_initial", initial = "y" }),
                this.Url.Route("WithoutExtension", new { name = "member_current" }),
                this.Url.Route("WithoutExtension", new { name = "member_current_by_initial", initial = "z" }),
                this.Url.Route("WithoutExtension", new { name = "member_a_to_z" }),
                this.Url.Route("WithoutExtension", new { name = "member_current_a_to_z" }),

                this.Url.Route("WithoutExtension", new { name = "constituency_index" }),
                this.Url.Route("WithoutExtension", new { name = "constituency_by_id", constituency_id = helpIds["Vale of Glamorgan"] }),
                this.Url.Route("WithoutExtension", new { name = "constituency_map", constituency_id = helpIds["Vale of Glamorgan"] }),
                this.Url.Route("WithoutExtension", new { name = "constituency_by_initial", initial = "l" }),
                this.Url.Route("WithoutExtension", new { name = "constituency_current" }),
                this.Url.Route("WithoutExtension", new { name = "constituency_lookup", property = "onsCode", value = "E14000699" }),
                this.Url.Route("WithoutExtension", new { name = "constituency_by_substring", substring = "heath" }),
                this.Url.Route("WithoutExtension", new { name = "constituency_current_by_initial", initial = "v" }),
                this.Url.Route("WithoutExtension", new { name = "constituency_a_to_z" }),
                this.Url.Route("WithoutExtension", new { name = "constituency_current_a_to_z" }),
                this.Url.Route("WithoutExtension", new { name = "constituency_members", constituency_id = helpIds["Vale of Glamorgan"] }),
                this.Url.Route("WithoutExtension", new { name = "constituency_current_member", constituency_id = helpIds["Vale of Glamorgan"] }),
                this.Url.Route("WithoutExtension", new { name = "constituency_contact_point", constituency_id = helpIds["Vale of Glamorgan"] }),
                this.Url.Route("WithoutExtension", new { name = "constituency_lookup_by_postcode", postcode = "sw1p 3ja" }),
                this.Url.Route("WithoutExtension", new { name = "find_your_constituency" }),

                this.Url.Route("WithoutExtension", new { name = "party_index" }),
                this.Url.Route("WithoutExtension", new { name = "party_by_id", party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "party_by_initial", initial = "a" }),
                this.Url.Route("WithoutExtension", new { name = "party_current" }),
                this.Url.Route("WithoutExtension", new { name = "party_by_substring", substring = "lock" }),
                this.Url.Route("WithoutExtension", new { name = "party_a_to_z" }),
                this.Url.Route("WithoutExtension", new { name = "party_current_a_to_z" }),
                this.Url.Route("WithoutExtension", new { name = "party_lookup", property = "mnisId", value = "231" }),
                this.Url.Route("WithoutExtension", new { name = "party_members", party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "party_current_members", party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "party_members_by_initial", party_id = helpIds["Labour"], initial = "f" }),
                this.Url.Route("WithoutExtension", new { name = "party_members_a_to_z", party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "party_current_members_by_initial", party_id = helpIds["Labour"], initial = "z" }),
                this.Url.Route("WithoutExtension", new { name = "party_current_members_a_to_z", party_id = helpIds["Labour"] }),

                this.Url.Route("WithoutExtension", new { name = "house_index"}),
                this.Url.Route("WithoutExtension", new { name = "house_by_id", house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "house_lookup", property = "name", value = "House of Lords" }),
                this.Url.Route("WithoutExtension", new { name = "house_by_substring", substring = "house" }),
                this.Url.Route("WithoutExtension", new { name = "house_members", house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "house_current_members", house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "house_parties", house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "house_current_parties", house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "house_party_by_id", house_id = helpIds["House Of Commons"], party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "house_members_by_initial", house_id = helpIds["House Of Commons"], initial = "m" }),
                this.Url.Route("WithoutExtension", new { name = "house_members_a_to_z", house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "house_current_members_by_initial", house_id = helpIds["House Of Commons"], initial = "m" }),
                this.Url.Route("WithoutExtension", new { name = "house_current_members_a_to_z", house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "house_party_members", house_id = helpIds["House Of Commons"], party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "house_party_members_by_initial", house_id = helpIds["House Of Commons"], party_id = helpIds["Labour"], initial = "c" }),
                this.Url.Route("WithoutExtension", new { name = "house_party_members_a_to_z", house_id = helpIds["House Of Commons"], party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "house_party_current_members", house_id = helpIds["House Of Commons"], party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "house_party_current_members_by_initial", house_id = helpIds["House Of Commons"], party_id = helpIds["Labour"], initial = "f" }),
                this.Url.Route("WithoutExtension", new { name = "house_party_current_members_a_to_z", house_id = helpIds["House Of Commons"], party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "house_committees_index", house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "house_committees_a_to_z", house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "house_committees_by_initial", house_id = helpIds["House Of Commons"], initial = "h" }),
                this.Url.Route("WithoutExtension", new { name = "house_current_committees", house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "house_current_committees_a_to_z", house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "house_current_committees_by_initial", house_id = helpIds["House Of Commons"], initial = "h" }),

                this.Url.Route("WithoutExtension", new { name = "contact_point_index" }),
                this.Url.Route("WithoutExtension", new { name = "contact_point_by_id", contact_point_id = helpIds["Yeovil Lib Dems Contact"] }),

                this.Url.Route("WithoutExtension", new { name = "parliament_index" }),
                this.Url.Route("WithoutExtension", new { name = "parliament_current" }),
                this.Url.Route("WithoutExtension", new { name = "parliament_next" }),
                this.Url.Route("WithoutExtension", new { name = "parliament_previous" }),
                this.Url.Route("WithoutExtension", new { name = "parliament_lookup", property = "parliamentPeriodNumber", value = "56" }),
                this.Url.Route("WithoutExtension", new { name = "parliament_by_id", parliament_id = helpIds["56th Parliament"] }),
                this.Url.Route("WithoutExtension", new { name = "next_parliament_by_id", parliament_id = helpIds["56th Parliament"] }),
                this.Url.Route("WithoutExtension", new { name = "previous_parliament_by_id", parliament_id = helpIds["56th Parliament"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_members", parliament_id = helpIds["56th Parliament"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_members_by_initial", parliament_id = helpIds["56th Parliament"], initial = "d" }),
                this.Url.Route("WithoutExtension", new { name = "parliament_members_a_to_z", parliament_id = helpIds["56th Parliament"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_houses", parliament_id = helpIds["56th Parliament"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_house", parliament_id = helpIds["56th Parliament"], house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_house_members", parliament_id = helpIds["56th Parliament"], house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_house_members_a_to_z", parliament_id = helpIds["56th Parliament"], house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_house_members_by_initial", parliament_id = helpIds["56th Parliament"], house_id = helpIds["House Of Commons"], initial = "d" }),
                this.Url.Route("WithoutExtension", new { name = "parliament_parties", parliament_id = helpIds["56th Parliament"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_party", parliament_id = helpIds["56th Parliament"], party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_party_members", parliament_id = helpIds["56th Parliament"], party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_party_members_a_to_z", parliament_id = helpIds["56th Parliament"], party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_party_members_by_initial", parliament_id = helpIds["56th Parliament"], party_id = helpIds["Labour"], initial = "d" }),
                this.Url.Route("WithoutExtension", new { name = "parliament_house_parties", parliament_id = helpIds["56th Parliament"], house_id = helpIds["House Of Commons"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_house_party", parliament_id = helpIds["56th Parliament"], house_id = helpIds["House Of Commons"], party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_house_party_members", parliament_id = helpIds["56th Parliament"], house_id = helpIds["House Of Commons"], party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_house_party_members_a_to_z", parliament_id = helpIds["56th Parliament"], house_id = helpIds["House Of Commons"], party_id = helpIds["Labour"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_house_party_members_by_initial", parliament_id = helpIds["56th Parliament"], house_id = helpIds["House Of Commons"], party_id = helpIds["Labour"], initial = "d" }),
                this.Url.Route("WithoutExtension", new { name = "parliament_constituencies", parliament_id = helpIds["56th Parliament"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_constituencies_a_to_z", parliament_id = helpIds["56th Parliament"] }),
                this.Url.Route("WithoutExtension", new { name = "parliament_constituencies_by_initial", parliament_id = helpIds["56th Parliament"], initial = "d" }),

                this.Url.Route("WithoutExtension", new { name = "image_by_id", image_id = "qnsCGpnw" }),

                this.Url.Route("WithoutExtension", new { name = "region_index" }),
                this.Url.Route("WithoutExtension", new { name = "region_by_id", region_code = "E15000001" }),
                this.Url.Route("WithoutExtension", new { name = "region_constituencies", region_code = "E15000001" }),
                this.Url.Route("WithoutExtension", new { name = "region_constituencies_a_to_z", region_code = "E15000001" }),
                this.Url.Route("WithoutExtension", new { name = "region_constituencies_by_initial", region_code = "E15000001", initial = "h" }),

                this.Url.Route("WithoutExtension", new { name = "formal_body_index" }),
                this.Url.Route("WithoutExtension", new { name = "formal_body_by_id", formal_body_id = helpIds["Treasury Committee"] }),
                this.Url.Route("WithoutExtension", new { name = "formal_body_membership", formal_body_id = helpIds["Treasury Committee"] }),

                this.Url.Route("WithoutExtension", new { name = "person_by_mnis_id", person_mnis_id = "185" }),
                this.Url.Route("WithoutExtension", new { name = "constituency_current_by_proximity_to_point", border_range = "10", centre_point_range = "20", latitude = "51.496130", longitude = "-0.125718" }),
                this.Url.Route("WithoutExtension", new { name = "webarticle_by_id", webarticle_id = helpIds["How to submit a written question online"]}),
                this.Url.Route("WithoutExtension", new { name = "concept_by_id", concept_id = helpIds["Oral questions"]}),
                this.Url.Route("WithoutExtension", new { name = "concept_index"})
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
