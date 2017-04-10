namespace Parliament.Data.Api.FixedQuery.Controllers
{
    using System.Net;
    using System.Web.Http;

    public class BadRequestController : ApiController
    {
        public object Get()
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }
    }
}