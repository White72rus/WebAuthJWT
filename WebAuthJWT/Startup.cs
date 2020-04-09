using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebAuthJWT.Controllers;
using WebAuthJWT.JWT;
using WebAuthJWT.JWT.Middleware.Extensions;

namespace WebAuthJWT
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                    // Использовать ли HTTPS при отправке токена
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = true,

                        IssuerSigningKey = AuthOptions.GetSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                }).AddCookie(config => {
                    config.LoginPath = "/auth";
                });

            services.AddCors();
            services.AddMvc(option => {
                option.EnableEndpointRouting = false;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors(x => x
                .WithOrigins("https://localhost:5001")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always,
            });

            app.UseCookiesJwt();

            //app.Use(async (context, next) => {
            //    // Защита от уязвимостей типа MIME sniffing
            //    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            //    // Защита от уязвимости XSS
            //    context.Response.Headers.Add("X-Xss-Protection", "1");
            //    // Защита от попыток clickjacking-взлома, не работать в нутри HTML-фрейма
            //    context.Response.Headers.Add("X-Frame-Options", "DENY");

            //    await next.Invoke();
            //});

            //app.Use(async (context, next) => {
            //    var path = context.Request.Path.Value.ToLower();
            //    var token = context.Request.Cookies["ANC.App.Conf.Id"];
            //    var bools = (string.Compare(path, "/auth", StringComparison.CurrentCultureIgnoreCase) == 0);

            //    await next.Invoke();
            //});

            //app.Use(async (context, next) => {
            //    var path = context.Request.Path.Value.ToLower();
            //    var token = context.Request.Cookies["ANC.App.Conf.Id"];
            //    var bools = (string.Compare(path, "/auth", StringComparison.CurrentCultureIgnoreCase) == 0);
            //    if (!string.IsNullOrEmpty(token) && !bools)
            //    {
            //        context.Request.Headers.Add("Authorization", "Bearer " + token);
            //        if (path != "/auth")
            //        {
            //            //context.Response.Redirect("/auth");
            //        }
            //    }
            //    else if (string.IsNullOrEmpty(token) && path != "/auth")
            //    {
            //        context.Response.Redirect("/auth");
            //    }
            //    else
            //    {
            //        //context.Response.StatusCode = 401;
            //    }
            //    await next.Invoke();
            //});

            // Редирект по кукам.
            app.Use(async (context, next) =>
            {
                var cookie = context.Request.Cookies;
                var path = context.Request.Path;
                if (cookie.Count != 0)
                {
                    foreach (var item in cookie)
                    {
                        if (item.Key == "MyCook")
                        {
                            if (item.Value == "666" && path != "/")
                            {
                                context.Response.Redirect("/auth");
                            }
                        }
                        Console.WriteLine("Key: " + item.Key + "\tValue: " + item.Value);
                    }
                }
                await next.Invoke();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "auth",
                    template: "{controller=Auth}/{action=Index}");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                
            });
        }
    }
}
