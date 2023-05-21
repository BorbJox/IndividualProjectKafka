using Microsoft.IdentityModel.Tokens;
using StatisticsAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StatisticsAPI.Services
{
    public interface IJwtProvider
    {
        string Generate(User user);
    }

    public class JwtProvider : IJwtProvider
    {
        public string Generate(User user)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("adminkeythatsprettylong")),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(null, null, claims, null, null, signingCredentials);

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
    }
}
