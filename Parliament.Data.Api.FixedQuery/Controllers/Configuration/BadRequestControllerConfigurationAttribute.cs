namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System;
    using System.Web.Http.Controllers;

    public class BadRequestControllerConfigurationAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Services.Replace(typeof(IHttpActionSelector), new DefaultMethodSelector());
        }
    }
}
