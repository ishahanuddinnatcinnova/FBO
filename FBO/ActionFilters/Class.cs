using Microsoft.AspNetCore.Mvc.Filters;

namespace FBO.ActionFilters
{
    public class Class : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }
    }
}
