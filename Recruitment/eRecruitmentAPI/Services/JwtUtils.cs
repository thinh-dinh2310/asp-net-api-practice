using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using BusinessObject;
using Microsoft.IdentityModel.Tokens;
using Utils;

namespace eRecruitmentAPI.Services
{
    public class JwtToken
    {
        public JwtToken()
        {
        }

        public Claim[] GetClaims(User user) => new[] {
            new Claim("id", user.Id.ToString()),
            new Claim("email", user.Email ?? ""),
            new Claim("phone", user.Phone ?? ""),
            new Claim("roleId", user.RoleId.ToString()),
            new Claim("address", user.Address ?? ""),
            new Claim("displayName", user.DisplayName ?? ""),
        };

        public User GetUser(IEnumerable<Claim> claims) => new User()
        {
            Id = Guid.Parse(claims.First(c => c.Type == "id").Value),
            Email = claims.First(c => c.Type == "email").Value,
            Phone = claims.First(c => c.Type == "phone").Value,
            RoleId = Guid.Parse(claims.First(c => c.Type == "roleId").Value),
            Address = claims.First(c => c.Type == "address").Value,
            DisplayName = claims.First(c => c.Type == "displayName").Value,
        };

        public string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppConfiguration.GetAppsetting("Tokens", "SecretAccessToken"));
            Console.WriteLine(AppConfiguration.GetAppsetting("Tokens", "SecretAccessToken"), AppConfiguration.GetAppsetting("Tokens", "SecretRefreshToken"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(GetClaims(user)),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppConfiguration.GetAppsetting("Tokens", "SecretRefreshToken"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(GetClaims(user)),
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public User ValidateAccessToken(string token)
        {
            if (token == null || token == "")
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppConfiguration.GetAppsetting("Tokens", "SecretAccessToken"));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                return GetUser(jwtToken.Claims);
            }
            catch (SecurityTokenExpiredException ex)
            {
                throw new SecurityTokenExpiredException("JWT Expired!");
            }
            catch
            {
                return null;
            }
        }

        public User ValidateRefreshToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(AppConfiguration.GetAppsetting("Tokens", "SecretRefreshToken"));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                return GetUser(jwtToken.Claims);
            }
            catch
            {
                return null;
            }
        }
    }
}
