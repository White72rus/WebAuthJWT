using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAuthJWT.JWT.Middleware
{
    public class UseCookiesJwt
    {
        private readonly RequestDelegate next;

        public UseCookiesJwt(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Защита от уязвимостей типа MIME sniffing
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            // Защита от уязвимости XSS
            context.Response.Headers.Add("X-Xss-Protection", "1");
            // Защита от попыток clickjacking-взлома, не работать в нутри HTML-фрейма
            context.Response.Headers.Add("X-Frame-Options", "DENY");

            var path = context.Request.Path.Value.ToLower();

            var token = context.Request.Cookies["ANC.App.Conf.Id"];

            var bools = (string.Compare(path, "/auth", StringComparison.CurrentCultureIgnoreCase) == 0);

            if (context.Request.Method != "POST")
            {
                if (string.IsNullOrEmpty(token) && path != "/auth")
                    context.Response.Redirect("/auth");
            }
            

            if (!string.IsNullOrEmpty(token) && !bools)
            {
                // Устанавливаем хедер авторизации
                context.Request.Headers.Add("Authorization", "Bearer " + token);
            }
            else if (!(context.Response.StatusCode != 301 || context.Response.StatusCode != 302))
            {
                context.Response.StatusCode = 401;
            }
            await next(context);
            // Before begin code
        }
    }
}
