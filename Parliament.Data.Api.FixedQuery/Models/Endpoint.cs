namespace Parliament.Data.Api.FixedQuery.Models
{
    using System.Collections.Generic;

    public class Endpoint
    {
        public EndpointType Type;
        public Dictionary<string, ParameterType> Parameters;
    }
}