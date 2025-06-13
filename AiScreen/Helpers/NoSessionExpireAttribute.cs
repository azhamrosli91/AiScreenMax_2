using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MaxSys.Helpers
{
    public class NoSessionExpireAttribute : ActionFilterAttribute
    {

        public NoSessionExpireAttribute()
        {

        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor != null)
            {
                var hasAllowAnonymous = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();
                var hasNoSessionExpire = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(NoSessionExpireAttribute), true).Any();

                if (hasAllowAnonymous || hasNoSessionExpire)
                {
                    return;
                }
            }

            // Your session expiration logic here
            // Example:
            if (context.HttpContext.Session.GetString("SessionKey") == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }

            base.OnActionExecuting(context);

        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //_logger.LogInformation($"Action {context.ActionDescriptor.DisplayName} executed in {executionTime} ms.");
        }
    }
}
