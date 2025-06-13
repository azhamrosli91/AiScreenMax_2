using Microsoft.AspNetCore.Mvc.Filters;

namespace MaxSys.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class BaseAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Your custom logic here
            // For example, check if the user has certain permissions or perform some other validation.
            base.OnActionExecuting(context);
        }
    }

}
