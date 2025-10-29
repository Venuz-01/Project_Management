using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Project_Management_System.Filter
{
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _role;

        public AuthorizeRoleAttribute(string role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userRole = context.HttpContext.Request.Headers["UserRole"].ToString();

            if (string.IsNullOrEmpty(userRole) || !userRole.Equals(_role, StringComparison.OrdinalIgnoreCase))
            {
                // Replace ForbidResult() with a simple 403 StatusCodeResult
                context.Result = new StatusCodeResult(403); // Forbidden
            }
        }
    }
}
