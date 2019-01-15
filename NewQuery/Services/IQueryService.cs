namespace NewQuery
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public interface IQueryService
    {
        IActionResult Execute(string name, IQueryCollection parameters);
    }
}
