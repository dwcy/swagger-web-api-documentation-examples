using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ITHS.Webapi
{
    public class Authentication
    {
        public Authentication()
        {
        }
        
        public string CreateJWTBearerToken(string serverHostUrl)
        {
            var key = GetSymmetricSecurityKey();
            var signinCredentials = DoDigitalSigningUsingTheKey(key);
            var jwtSecurityToken = CreateJwtSecurityToken(signinCredentials, serverHostUrl);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        private SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            var myKeyInConfig = "ITHSsuperSecretKey";
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(myKeyInConfig));
        }

        private SigningCredentials DoDigitalSigningUsingTheKey(SymmetricSecurityKey key)
        {
            var signinCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            return signinCredentials;
        }
        private List<Claim> GetClaims()
        {
            var claims = new List<Claim>
            {
                    new Claim("School", "ITHS"),
                    new Claim("Teacher", "Dawid")
            };
            return claims;
        }

        public JwtSecurityToken CreateJwtSecurityToken(SigningCredentials signinCredentials, string serverHostUrl)
        {
            var issuerFromConfig = "ITHS";
            var expireDate = DateTime.Now.AddMinutes(5);
            var claims = GetClaims();

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: issuerFromConfig,
                audience: serverHostUrl,
                claims: claims,
                expires: expireDate,
                signingCredentials: signinCredentials
            );

            return jwtSecurityToken;
        }
    }
}
