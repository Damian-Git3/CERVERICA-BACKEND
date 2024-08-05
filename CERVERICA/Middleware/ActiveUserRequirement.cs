using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CERVERICA.Middleware
{
    public class ActiveUserRequirement : IAuthorizationRequirement
    {
        public ActiveUserRequirement()
        {
        }
    }

    public class ActiveUserHandler : AuthorizationHandler<ActiveUserRequirement>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ActiveUserHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveUserRequirement requirement)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user != null && user.Activo)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
