namespace Parliament.Data.Api.FixedQuery
{
    using System.Collections.Generic;

    public class Endpoint
    {
        public EndpointType Type { get; set; }

        public Dictionary<string, ParameterType> Parameters { get; set; }
    }
}