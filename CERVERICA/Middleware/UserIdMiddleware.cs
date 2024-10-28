using System.Security.Claims;

namespace CERVERICA.Middleware
{
    public class UserIdMiddleware
    {
        private readonly RequestDelegate _next;

        public UserIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var idUsuario = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(idUsuario))
            {
                context.Items["idUsuario"] = idUsuario;
            }

            await _next(context);
        }
    }
}
