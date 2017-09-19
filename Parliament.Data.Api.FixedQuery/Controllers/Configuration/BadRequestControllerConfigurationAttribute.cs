namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http.Controllers;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BadRequestControllerConfigurationAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Services.Replace(typeof(IHttpActionSelector), new DefaultMethodSelector());
        }
    }
}
