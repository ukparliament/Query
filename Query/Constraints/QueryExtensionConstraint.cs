namespace Query
{
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    internal class QueryExtensionConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var extension = values[routeKey] as string;

            return Configuration.QueryMappings.Any(mapping => mapping.Extensions.Contains(extension));
        }
    }
}
