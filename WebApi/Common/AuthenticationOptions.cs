using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Common
{
    public class AuthenticationOptions
    {
        public const string Issuer = "BookeryServer";
        public const string Audience = "BookeryClient";
        public const int AccessTokenExpiration = 600;
        public const int RefreshTokenExpiration = 3000;

        // must be stored in a secure place
        private const string Key = "abcdefd12345!$%";

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.Unicode.GetBytes(Key));
        }
    }
}