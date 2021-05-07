using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Common
{
    public class AuthenticationOptions
    {
        public const string Issuer = "BookeryServer";
        public const string Audience = "BookeryClient";
        public const int AccessTokenExpiration = 60;
        public const int RefreshTokenExpiration = 300;

        private const string Key = "abcdefd12345!$%";

        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.Unicode.GetBytes(Key));
    }
}
