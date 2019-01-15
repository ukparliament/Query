namespace NewQuery
{
    using System.Reflection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    internal class HardcodedQueryService : QueryService
    {
        private delegate IActionResult Action(IQueryCollection parameters);

        public override IActionResult Execute(string name, IQueryCollection parameters)
        {
            var method = typeof(HardcodedQueryService).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);

            if (method is null)
            {
                return base.Execute(name, parameters);
            }
            
            var actionMethod = (Action)method.CreateDelegate(typeof(Action), this);

            return actionMethod(parameters);
        }

        private IActionResult b(IQueryCollection parameters)
        {
            return new OkObjectResult("B");
        }
    }
}
