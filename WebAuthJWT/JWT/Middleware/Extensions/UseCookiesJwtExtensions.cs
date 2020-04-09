using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAuthJWT.JWT.Middleware.Extensions
{
    public static class UseCookiesJwtExtensions
    {
        public static IApplicationBuilder UseCookiesJwt(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UseCookiesJwt>();
        }
    }
}
