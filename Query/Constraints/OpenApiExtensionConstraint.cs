namespace Query
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.OpenApi;

    internal class OpenApiExtensionConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return Enum.GetNames(typeof(OpenApiFormat)).Contains((string)values[routeKey], StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
