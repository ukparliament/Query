namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;
    using VDS.RDF;
    using VDS.RDF.Query;


    [RoutePrefix("parliaments")]

    public class ParliamentController : BaseController
    {
        // Ruby route: get '/parliaments'
        [Route("", Name = "ParliamentIndex")]
        [HttpGet]
        public Graph Index()
        {
            var queryString = @"
PREFIX : <http://id.ukpds.org/schema/>
CONSTRUCT {
    ?parliament
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate ;
        :parliamentPeriodEndDate ?endDate ;
        :parliamentPeriodNumber ?parliamentNumber .
}
WHERE {
    ?parliament 
        a :ParliamentPeriod ;
        :parliamentPeriodStartDate ?startDate ;
        :parliamentPeriodEndDate ?endDate ;
        :parliamentPeriodNumber ?parliamentNumber .
}
";

            var query = new SparqlParameterizedString(queryString);

            return BaseController.ExecuteList(query);
        }

//        // Ruby route: get '/parliaments/current', to: 'parliaments#current'
//        [Route(@"current", Name = "ParliamentCurrent")]
//        [HttpGet]
//        public Graph Current()
//        {
//            var queryString = @"
//PREFIX : <http://id.ukpds.org/schema/>
//CONSTRUCT {
//    ?parliament
//        a :ParliamentPeriod .
//}
//WHERE {
//    ?parliament a :ParliamentPeriod .
//    FILTER NOT EXISTS { ?parliament a :PastParliamentPeriod }
//}";

//            var query = new SparqlParameterizedString(queryString);

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: get '/parliaments/previous', to: 'parliaments#previous'
//        [Route(@"previous", Name = "ParliamentPrevious")]
//        [HttpGet]
//        public Graph Previous()
//        {
//            var queryString = @"
//PREFIX : <http://id.ukpds.org/schema/>
//CONSTRUCT {
//    ?previousParliament
//        a :ParliamentPeriod .
//}
//WHERE {
//    ?currentParliament a :ParliamentPeriod .
//    FILTER NOT EXISTS { ?currentParliament a :PastParliamentPeriod }
//    ?currentParliament :parliamentPeriodHasImmediatelyPreviousParliamentPeriod ?previousParliament .
//}";

//            var query = new SparqlParameterizedString(queryString);

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: get '/parliaments/next', to: 'parliaments#next'
//        [Route(@"previous", Name = "ParliamentNext")]
//        [HttpGet]
//        public Graph Next()
//        {
//            var queryString = @"";

//            var query = new SparqlParameterizedString(queryString);

//            return BaseController.ExecuteList(query);
//        }
//        // Ruby route: get '/parliaments/lookup', to: 'parliaments#lookup'
//        [Route(@"lookup/{source:regex(^\w+$)}/{id}", Name = "ParliamentLookup")]
//        [HttpGet]
//        public Graph Lookup(string source, string id)
//        {
//            var queryString = @"";
//            // Use @id, @source
//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("source", new Uri(BaseController.schema, source));
//            query.SetLiteral("id", id);

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: match '/parliaments/:parliament', to: 'parliaments#show', parliament: /\w{8}/, via: [:get]
//        [Route(@"{id:regex(^\w{8}$)}", Name = "ParliamentById")]
//        [HttpGet]
//        public Graph ById(string id)
//        {
//            var queryString = @"";
//            // Use @id
//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("id", new Uri(BaseController.instance, id));

//            return BaseController.ExecuteSingle(query);

//        }

//        // Ruby route: get '/parliaments/:parliament/members', to: 'parliaments#members'
//        [Route(@"{id:regex(^\w{8}$)}/members", Name = "ParliamentMembers")]
//        [HttpGet]
//        public Graph Members(string id)
//        {
//            var queryString = @"";
//            // Use @parliamentid
//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, id));

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: match '/parliaments/:parliament/members/:letter', to: 'parliaments#members_letters', letter: /[A-Za-z]/, via: [:get]
//        [Route(@"{parliamentid:regex(^\w{8}$)}/members/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ParliamentMembersByInitial")]
//        [HttpGet]
//        public Graph MembersByInitial(string parliamentid, string initial)
//        {
//            var queryString = @"";
//            // Use @parliamentid, @initial

//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, parliamentid));
//            query.SetLiteral("initial", initial);

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: get '/parliaments/:parliament/members/a_z_letters', to: 'parliaments#a_z_letters_members'
//        [Route(@"{id:regex(^\w{8}$)}/members/a_z_letters", Name = "ParliamentMembersAToZ")]
//        [HttpGet]
//        public Graph MembersAToZLetters(string id)
//        {
//            var queryString = @"";
//            // Use @parliamentid
//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, id));

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: get '/parliaments/:parliament/members/houses', to: 'parliaments#members_houses'
//        [Route(@"{id:regex(^\w{8}$)}/members/houses", Name = "ParliamentMembersHouses")]
//        [HttpGet]
//        public Graph MembersHouses(string id)
//        {
//            var queryString = @"";
//            // Use @parliamentid
//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, id));

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: get '/parliaments/:parliament/members/houses/:house', to: 'parliaments#members_house'
//        [Route(@"{parliamentid:regex(^\w{8}$)}/members/houses/{houseid:regex(^\w{8}$)}", Name = "ParliamentMembersHouse")]
//        [HttpGet]
//        public Graph MembersHouse(string parliamentid, string houseid)
//        {
//            var queryString = @"";
//            // Use @parliamentid, @houseid
//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, parliamentid));
//            query.SetUri("houseid", new Uri(BaseController.instance, houseid));

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: get '/parliaments/:parliament/members/houses/:house/a_z_letters', to: 'parliaments#a_z_letters_members_house'
//        [Route(@"{parliamentid:regex(^\w{8}$)}/members/houses/{houseid:regex(^\w{8}$)}/a_z_letters", Name = "ParliamentMembersHouseAToZ")]
//        [HttpGet]
//        public Graph MembersHouseAToZ(string parliamentid, string houseid)
//        {
//            var queryString = @"";
//            // Use @parliamentid, @houseid
//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, parliamentid));
//            query.SetUri("houseid", new Uri(BaseController.instance, houseid));

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: match '/parliaments/:parliament/members/houses/:house/:letter', to: 'parliaments#members_house_letters', letter: /[A-Za-z]/, via: [:get]
//        [Route(@"{parliamentid:regex(^\w{8}$)}/members/houses/{houseid:regex(^\w{8}$)}/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ParliamentMembersHouseByInitial")]
//        [HttpGet]
//        public Graph MembersHouse(string parliamentid, string houseid, string initial)
//        {
//            var queryString = @"";
//            // Use @parliamentid, @houseid, @initial
//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, parliamentid));
//            query.SetUri("houseid", new Uri(BaseController.instance, houseid));
//            query.SetLiteral("initial", initial);

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: get '/parliaments/:parliament/parties', to: 'parliaments#parties'
//        [Route(@"{id:regex(^\w{8}$)}/parties", Name = "ParliamentParties")]
//        [HttpGet]
//        public Graph Parties(string id)
//        {
//            var queryString = @"";
//            // Use @parliamentid

//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, id));

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: get '/parliaments/:parliament/parties/houses', to: 'parliaments#parties_houses'
//        [Route(@"{id:regex(^\w{8}$)}/parties/houses", Name = "ParliamentPartiesHouses")]
//        [HttpGet]
//        public Graph PartiesHouses(string id)
//        {
//            var queryString = @"";
//            // Use @parliamentid

//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, id));

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: get '/parliaments/:parliament/parties/houses/:house', to: 'parliaments#parties_house'
//        [Route(@"{parliamentid:regex(^\w{8}$)}/parties/houses/{houseid:regex(^\w{8}$)}", Name = "ParliamentPartiesHouse")]
//        [HttpGet]
//        public Graph PartiesHouse(string parliamentid, string houseid)
//        {
//            var queryString = @"";
//            // Use @parliamentid, @houseid

//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, parliamentid));
//            query.SetUri("houseid", new Uri(BaseController.instance, houseid));

//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: get '/parliaments/:parliament/constituencies', to: 'parliaments#constituencies'
//        [Route(@"{id:regex(^\w{8}$)}/constituencies", Name = "ParliamentConstituencies")]
//        [HttpGet]
//        public Graph Constituencies(string id)
//        {
//            var queryString = @"";
//            // Use @parliamentid

//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, id));
//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: get '/parliaments/:parliament/constituencies/a_z_letters', to: 'parliaments#a_z_letters_constituencies'
//        [Route(@"{id:regex(^\w{8}$)}/constituencies/a_z_letters", Name = "ParliamentConstituenciesAToZ")]
//        [HttpGet]
//        public Graph ConstituenciesAToZ(string id)
//        {
//            var queryString = @"";
//            // Use @parliamentid

//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, id));
//            return BaseController.ExecuteList(query);
//        }

//        // Ruby route: match '/parliaments/:parliament/constituencies/:letter', to: 'parliaments#constituencies_letters', letter: /[A-Za-z]/, via: [:get]
//        [Route(@"{id:regex(^\w{8}$)}/constituencies/{initial:regex(^\p{L}+$):maxlength(1)}", Name = "ParliamentConstituenciesByInitial")]
//        [HttpGet]
//        public Graph ConstituenciesAToZ(string id, string initial)
//        {
//            var queryString = @"";
//            // Use @parliamentid, @initial

//            var query = new SparqlParameterizedString(queryString);

//            query.SetUri("parliamentid", new Uri(BaseController.instance, id));
//            query.SetLiteral("initial", initial);
//            return BaseController.ExecuteList(query);
//        }
    }
}