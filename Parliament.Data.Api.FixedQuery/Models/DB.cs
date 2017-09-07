namespace Parliament.Data.Api.FixedQuery.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class DB
    {
        [JsonProperty("$schema")]
        public string Schema;

        public Dictionary<string, Endpoint> Endpoints;
    }
}