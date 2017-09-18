namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Web.Http;

    [BadRequestControllerConfiguration]
    public class BadRequestController : ApiController
    {
        public IHttpActionResult Default()
        {
            return this.BadRequest();
        }
    }
}
