namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Web.Http;
    using System.Web.Http.Description;

    [BadRequestControllerConfiguration]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BadRequestController : ApiController
    {
        public IHttpActionResult Default()
        {
            return this.BadRequest();
        }
    }
}
