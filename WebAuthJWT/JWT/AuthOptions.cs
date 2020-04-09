using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace WebAuthJWT.JWT
{
    public class AuthOptions
    {
        public const string ISSUER = "W_Auth_Serwer";
        public const string AUDIENCE = "W_Auth_Client";
        public static int lifeTime = 5;

        private static string key = KeyGen.Generate(256, GenFlags.LOWER_CH | GenFlags.DIGIT_CH);
        
        /// <summary>
        /// Getting symmetrical security secret key.
        /// </summary>
        /// <returns></returns>
        public static SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }
    }
}
