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
        readonly IUserService _userService;
        public JwtProvider(IUserService userService)
        {
            _userService = userService;
        }
        public string Generate(User user)
        {
            var claims = new Claim[]
            {
                new Claim("Name", user.Name),
                new Claim("IsAdmin", user.IsAdmin ? "1" : "0"),
            };

            User? foundUser = _userService.GetByName(user.Name) ?? throw new Exception("No such user");

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(foundUser.APIKey)),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(null, null, claims, null, null, signingCredentials);

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
    }
}
