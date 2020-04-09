using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAuthJWT.DataBase;
using WebAuthJWT.JWT;

namespace WebAuthJWT.Controllers
{
    public class AuthController : Controller
    {
        private List<Person> people = new List<Person>
        {
            new Person{Name="Андре", Login="andre-fd", Mail="andre-fd@ya.ru", Role="user", Pass="qwerty"},
            new Person{Name="Ешуа", Login="god-jo", Mail="god-jo@ya.ru", Role="admin", Pass="369sedhuj"},
            new Person{Name="Борис", Login="boris-jo", Mail="boris-jo@ya.ru", Role="user", Pass="123"},
        };

        [Route("/Auth", Name = "auth")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/token")]
        public IActionResult Token(string username, string password)
        {
            var identity = GetIdentity(username, password);

            if (identity == null)
            {
                return BadRequest(new { erorrText = "Invalid username or password." });
            }

            var date = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: date,
                claims: identity.Claims,
                expires: date.Add(TimeSpan.FromMinutes(AuthOptions.lifeTime)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSecurityKey(), SecurityAlgorithms.HmacSha256)
                );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            SetCookiesToken(encodedJwt);

            var response = new
            {
                access_token = encodedJwt,
                user_name = identity.Name,
            };

            return Json(response);
        }

        private ClaimsIdentity GetIdentity(string login, string password)
        {
            Person person = people.FirstOrDefault(o => o.Login == login && o.Pass == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Name),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role),
                    new Claim(ClaimTypes.Email, person.Mail),
                };
                return new ClaimsIdentity(claims, "Token",
                    ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);   
            }
            return null;
        }
        private void SetCookiesToken(string token) {
            var cookieOptions = new CookieOptions
            {
                MaxAge = TimeSpan.FromMinutes(5)
            };

            HttpContext.Response.Cookies.Append("ANC.App.Conf.Id", token, cookieOptions);
        }
    }
}
