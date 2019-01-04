// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace FriendlyHierarchyTests
{
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using VDS.RDF;
    using VDS.RDF.Parsing;

    public static class FixedQueryHelper
    {
        private static string aiApp;
        private static string aiKey;
        private static string cloudRoleName;
        private static Uri fixedQueryEndpoint;
        private static TimeSpan aiHours;

        public static IDictionary Properties
        {
            set
            {
                aiApp = value["AIApp"] as string;
                aiKey = value["AIKey"] as string;
                cloudRoleName = value["FixedQueryCloudRoleName"] as string;
                fixedQueryEndpoint = new Uri(value["FixedQueryEndpoint"] as string);
                aiHours = TimeSpan.FromHours(double.Parse(value["AIHours"] as string));
            }
        }

        static FixedQueryHelper()
        {
            UriLoader.Cache.Clear();
            Options.UriLoaderCaching = false;
        }

        public static IEnumerable<object[]> All
        {
            get
            {
                return QueryAI().Result.tables[0].rows
                    .Skip(50)
                    .Take(100)
                    .Select(row => row[0])
                    .Select(url => new Uri(url))
                    .Select(uri => uri.PathAndQuery)
                    .Select(path => path.StartsWith("/") ? path.Substring(1) : path)
                    .Select(path => new[] { path });
            }
        }

        public static IEnumerable<object[]> Specific
        {
            get
            {
                return new[] {
                    //new[] {"constituency_current" },
                    //new[] {"constituency_index" },
                    //new[] {"contact_point_index" },
                    //new[] {"formal_body_index" },
                    //new[] {"member_current" },
                    //new[] {"member_index" },
                    //new[] {"person_index" },
                    //new[] {"photo_details?photo_id=H74fzQJf" },

                    new[] {"constituency_a_to_z" },
                    new[] {"constituency_by_id?constituency_id=HTt25jB3" },
                    new[] {"constituency_by_initial?initial=a" },
                    new[] {"constituency_current_a_to_z" },
                    new[] {"constituency_current_by_initial?initial=a" },
                    new[] {"constituency_current_member?constituency_id=3WLS0fFd" },
                    new[] {"constituency_lookup_by_postcode?postcode=SE155PY" },
                    new[] {"constituency_map?constituency_id=VBFKxifJ" },
                    new[] {"constituency_members?constituency_id=3WLS0fFd" },
                    new[] {"contact_point_by_id?contact_point_id=wk1atnfh" },
                    new[] {"exists?uri=https://id.parliament.uk/43RHonMf" },
                    new[] {"find_your_constituency" },
                    new[] {"formal_body_by_id?formal_body_id=tz34m7Vt" },
                    new[] {"formal_body_index" },
                    new[] {"formal_body_membership?formal_body_id=tz34m7Vt" },
                    new[] {"government_position_index" },
                    new[] {"group_a_to_z" },
                    new[] {"group_by_id?group_id=BuhjfUz0" },
                    new[] {"group_by_initial?group_id=BuhjfUz0&initial=p" },
                    new[] {"group_by_substring?substring=army" },
                    new[] {"group_current" },
                    new[] {"group_houses_index?group_id=BuhjfUz0" },
                    new[] {"group_index" },
                    new[] {"group_memberships_a_to_z?group_id=BuhjfUz0" },
                    new[] {"group_memberships_by_initial?group_id=BuhjfUz0&initial=g" },
                    new[] {"group_memberships_index?group_id=BuhjfUz" },
                    new[] {"group_positions_chairs_current?group_id=BuhjfUz" },
                    new[] {"group_positions_chairs_index?group_id=BuhjfUz" },
                    new[] {"group_positions_current?group_id=BuhjfUz" },
                    new[] {"group_positions_index?group_id=BuhjfUz" },
                    new[] {"group_questions_written_answered?group_id=XouN12Ow" },
                    new[] {"groups_committees_a_to_z" },
                    new[] {"groups_committees_by_initial?initial=p" },
                    new[] {"groups_committees_current_a_to_z" },
                    new[] {"groups_committees_current_by_initial?initial=a" },
                    new[] {"groups_committees_current" },
                    new[] {"groups_committees_index" },
                    new[] {"groups_government_organisation_a_to_z" },
                    new[] {"groups_government_organisation_by_initial?initial=d" },
                    new[] {"groups_government_organisation_current_a_to_z" },
                    new[] {"groups_government_organisation_current_by_initial?initial=d" },
                    new[] {"groups_government_organisation_current" },
                    new[] {"groups_government_organisation_index" },
                    new[] {"house_by_id?house_id=1AFu55Hs" },
                    new[] {"house_by_substring?substring=com" },
                    new[] {"house_committees_a_to_z?house_id=1AFu55Hs" },
                    new[] {"house_committees_by_initial?house_id=1AFu55Hs&initial=e" },
                    new[] {"house_committees_index?house_id=1AFu55Hs" },
                    new[] {"house_current_committees_a_to_z?house_id=1AFu55Hs" },
                    new[] {"house_current_committees_by_initial?house_id=1AFu55Hs&initial=r" },
                    new[] {"house_current_committees?house_id=1AFu55Hs" },
                    new[] {"house_current_members_a_to_z?house_id=1AFu55Hs" },
                    new[] {"house_current_members_by_initial?house_id=1AFu55Hs&initial=a" },
                    new[] {"house_current_members?house_id=WkUWUBMx" },
                    new[] {"house_current_parties?house_id=1AFu55Hs" },
                    new[] {"house_index" },
                    new[] {"house_members_a_to_z?house_id=1AFu55Hs" },
                    new[] {"house_members_by_initial?house_id=1AFu55Hs&initial=a" },
                    new[] {"house_members?house_id=1AFu55Hs" },
                    new[] {"house_parties?house_id=1AFu55Hs" },
                    new[] {"house_party_by_id?house_id=1AFu55Hs&party_id=DIifZMjq" },
                    new[] {"house_party_current_members?house_id=1AFu55Hs&party_id=1cIxOTTd" },
                    new[] {"house_party_current_members_a_to_z?house_id=1AFu55Hs&party_id=DIifZMjq" },
                    new[] {"house_party_current_members_by_initial?house_id=WkUWUBMx&party_id=wz637AXO&initial=s" },
                    new[] {"house_party_members_a_to_z?house_id=1AFu55Hs&party_id=DIifZMjq" },
                    new[] {"house_party_members_by_initial?house_id=1AFu55Hs&party_id=DIifZMjq&initial=z" },
                    new[] {"house_party_members?house_id=1AFu55Hs&party_id=DIifZMjq" },
                    new[] {"image_by_id?image_id=CGp8GLYv" },
                    new[] {"member_a_to_z" },
                    new[] {"member_by_initial?initial=a" },
                    new[] {"member_current_a_to_z" },
                    new[] {"member_current_by_initial?initial=a" },
                    new[] {"next_parliament_by_id?parliament_id=b0t56VVL" },
                    new[] {"parliament_by_id?parliament_id=Dhqf32aX" },
                    new[] {"parliament_constituencies?parliament_id=Dhqf32aX" },
                    new[] {"parliament_constituencies_by_initial?parliament_id=b0t56VVL&initial=h" },
                    new[] {"parliament_current" },
                    new[] {"parliament_house_members_a_to_z?parliament_id=b0t56VVL&house_id=1AFu55Hs" },
                    new[] {"parliament_house_members_by_initial.nt?parliament_id=b0t56VVL&house_id=1AFu55Hs&initial=a" },
                    new[] {"parliament_house_members_by_initial?parliament_id=b0t56VVL&house_id=1AFu55Hs&initial=a" },
                    new[] {"parliament_house_members?parliament_id=b0t56VVL&house_id=1AFu55Hs" },
                    new[] {"parliament_house_parties?parliament_id=b0t56VVL&house_id=1AFu55Hs" },
                    new[] {"parliament_house_party_members_a_to_z?parliament_id=b0t56VVL&house_id=1AFu55Hs&party_id=DIifZMjq" },
                    new[] {"parliament_house_party_members_by_initial?parliament_id=b0t56VVL&house_id=1AFu55Hs&party_id=DIifZMjq&initial=j" },
                    new[] {"parliament_house_party_members?parliament_id=b0t56VVL&house_id=1AFu55Hs&party_id=DIifZMjq&initial=j" },
                    new[] {"parliament_house_party?parliament_id=b0t56VVL&house_id=1AFu55Hs&party_id=DIifZMjq" },
                    new[] {"parliament_house?parliament_id=b0t56VVL&house_id=1AFu55Hs" },
                    new[] {"parliament_houses?parliament_id=b0t56VVL" },
                    new[] {"parliament_index" },
                    new[] {"parliament_members_a_to_z?parliament_id=b0t56VVL" },
                    new[] {"parliament_members_by_initial?parliament_id=b0t56VVL&initial=m" },
                    new[] {"parliament_members?parliament_id=b0t56VVL" },
                    new[] {"parliament_next" },
                    new[] {"parliament_parties?parliament_id=b0t56VVL" },
                    new[] {"parliament_party_members_a_to_z?parliament_id=b0t56VVL&party_id=LEYIBvV9" },
                    new[] {"parliament_party_members_by_initial?parliament_id=b0t56VVL&party_id=LEYIBvV9&initial=s" },
                    new[] {"parliament_party_members?parliament_id=b0t56VVL&party_id=LEYIBvV9" },
                    new[] {"parliament_party?parliament_id=b0t56VVL&party_id=LEYIBvV9" },
                    new[] {"parliament_previous" },
                    new[] {"party_a_to_z" },
                    new[] {"party_by_id?party_id=wnvdA00Y" },
                    new[] {"party_by_initial?initial=a" },
                    new[] {"party_current_a_to_z" },
                    new[] {"party_current_members_a_to_z?party_id=DIifZMjq" },
                    new[] {"party_current_members_by_initial?party_id=DIifZMjq&initial=c" },
                    new[] {"party_current_members?party_id=Q8zBFjn0" },
                    new[] {"party_current" },
                    new[] {"party_index" },
                    new[] {"party_members_a_to_z?party_id=GcWDomat" },
                    new[] {"party_members_by_initial?party_id=GcWDomat&initial=k" },
                    new[] {"party_members?party_id=GcWDomat" },
                    new[] {"person_a_to_z" },
                    new[] {"person_by_id?person_id=i8eCbE1j" },
                    new[] {"person_by_initial?initial=f" },
                    new[] {"person_by_mnis_id?person_mnis_id=3299" },
                    new[] {"person_committees_index?person_id=TyNGhslR" },
                    new[] {"person_committees_memberships_index?person_id=Fx1EcmX5" },
                    new[] {"person_constituencies?person_id=TyNGhslR" },
                    new[] {"person_contact_points?person_id=TyNGhslR" },
                    new[] {"person_current_committees_memberships?person_id=TyNGhslR" },
                    new[] {"person_current_constituency?person_id=TyNGhslR" },
                    new[] {"person_current_house?person_id=TyNGhslR" },
                    new[] {"person_current_party?person_id=TyNGhslR" },
                    new[] {"person_current_twitter" },
                    new[] {"person_houses?person_id=TyNGhslR" },
                    new[] {"person_index" },
                    new[] {"person_lookup?property=mnisId&value=403" },
                    new[] {"person_mps" },
                    new[] {"person_parties?person_id=TyNGhslR" },
                    new[] {"person_photo_index" },
                    new[] {"procedure_by_id?procedure_id=5S6p4YsP" },
                    new[] {"procedure_index" },
                    new[] {"proprosed_negative_statutory_instrument_by_id?proposed_negative_statutory_instrument_id=Tn1xqHc0"},
                    new[] {"proposed_negative_statutory_instrument_index"},
                    new[] {"question_by_id?question_id=s7kNrhvX" },
                    new[] {"questions_answeredby_member?member_id=aJ7Os4SE" },
                    new[] {"questions_askedby_member?member_id=aJ7Os4SE" },
                    new[] {"questions_search_by_title?lowercase_string=health" },
                    new[] {"region_by_id?region_code=W08000001" },
                    new[] {"region_constituencies?region_code=W08000001" },
                    new[] {"region_constituencies_by_initial?region_code=W08000001&initial=o" },
                    new[] {"region_index" },
                    new[] {"resource.nt?uri=https%3A%2F%2Fid.parliament.uk%2FDhqf32aX" },
                    new[] {"statutory_instrument_by_id?statutory_instrument_id=9PuVurua"},
                    new[] {"statutory_instrument_index"},
                    new[] {"webarticle_by_id?webarticle_id=8MHJ9zSp" },
                    new[] {"work_package_by_id?work_package_id=nFCm0vQC" },
                    new[] {"work_package_index" }, };
            }
        }

        public static IGraph x(string endpoint)
        {
            var g = new Graph();
            g.LoadFromUri(new Uri(fixedQueryEndpoint, endpoint));

            return g;
        }

        private async static Task<aiResponse> QueryAI()
        {
            var period = System.Xml.XmlConvert.ToString(aiHours);
            var aiApi = $"https://api.applicationinsights.io/v1/apps/{aiApp}/query?timespan={period}";
            var aiQuery = $@"requests | where cloud_RoleName == ""{cloudRoleName}"" | where success == true | distinct url";
            var query = JsonConvert.SerializeObject(new { query = aiQuery });

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-api-key", aiKey);

                using (var content = new StringContent(query))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    using (var response = await client.PostAsync(aiApi, content))
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<aiResponse>(body);
                    }
                }
            }
        }

    }

    class aiResponse
    {
        public table[] tables;
    }

    class table
    {
        public string name;
        public column[] columns;
        public string[][] rows;
    }

    class column
    {
        public string name;
        public string type;
    }
}
