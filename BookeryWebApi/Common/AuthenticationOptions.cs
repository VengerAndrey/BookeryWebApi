using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace BookeryWebApi.Common
{
    public class AuthenticationOptions
    {
        public const string Issuer = "BookeryServer";
        public const string Audience = "BookeryClient";
        public const int Lifetime = 20;

        private const string Key = "abcdefd12345!$%";

        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.Unicode.GetBytes(Key));
    }
}
