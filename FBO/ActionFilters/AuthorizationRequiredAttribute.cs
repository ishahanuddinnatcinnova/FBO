using FBO.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace FBO.ActionFilters
{
    public class AuthorizationRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Query["appId"].ToString() != null && filterContext.HttpContext.Request.Query["tokenId"].ToString() != null)
            {
                var appID = int.Parse(filterContext.HttpContext.Request.Query["appId"].ToString());
                var tokenValue = filterContext.HttpContext.Request.Query["tokenId"].ToString();

                // Validate Token
                if (!Token.Validate(appID, tokenValue))
                {
                    Log.Error("App ID: " + appID + ", Token: " + tokenValue + " Exception is: You need a valid token and appID to use this api. ");
                    filterContext.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                }
            }
            else
            {
                filterContext.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
