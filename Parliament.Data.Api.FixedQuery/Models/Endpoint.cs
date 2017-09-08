namespace Parliament.Data.Api.FixedQuery
{
    using System.Collections.Generic;

    public class Endpoint
    {
        public EndpointType Type;
        public Dictionary<string, ParameterType> Parameters;
    }
}