using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ITHS.Webapi
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)] //Rule where attribute can be applied. Class or method
    public class CustomAuthAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            var user = context.HttpContext.Items["User"];
            if (user == "test")
                context.Result = new JsonResult(new { message = "user name 'test' is forbidden!" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}
