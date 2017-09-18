namespace Parliament.Data.Api.FixedQuery
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Http.Routing;

    internal class EndpointConstraint : IHttpRouteConstraint
    {
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            var endpointName = values[parameterName] as string;

            return Resources.DB.Endpoints.ContainsKey(endpointName);
        }
    }
}