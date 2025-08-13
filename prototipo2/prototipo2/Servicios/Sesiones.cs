using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace prototipo2.Servicios
{
    public class Sesiones : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Session.GetString("JWT");

            if (string.IsNullOrEmpty(token) || TokenExpirado(token))
            {
                context.HttpContext.Session.Clear();
                context.Result = new RedirectToActionResult("InicioSesion", "Cliente", null);
            }
            else
            {
                base.OnActionExecuting(context);
            }
        }

        private bool TokenExpirado(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            if (!jwtHandler.CanReadToken(token))
                return true;

            var jwtToken = jwtHandler.ReadJwtToken(token);
            var exp = jwtToken.ValidTo;

            return exp < DateTime.UtcNow;
        }
    }




}

