namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Web.Http.Controllers;

    public class DefaultMethodSelector : ApiControllerActionSelector
    {
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            var defaultMethod = typeof(BadRequestController).GetMethod(nameof(BadRequestController.Default));
            return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, defaultMethod);
        }
    }

}
