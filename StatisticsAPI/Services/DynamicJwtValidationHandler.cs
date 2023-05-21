using Microsoft.IdentityModel.Tokens;
using StatisticsAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StatisticsAPI.Services
{
    public class DynamicJwtValidationHandler : JwtSecurityTokenHandler, ISecurityTokenValidator
    {
        readonly IServiceScopeFactory _serviceScopeFactory;

        public DynamicJwtValidationHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        private SecurityKey GetSSKeyForName(string name)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetService<IUserService>() ?? throw new Exception("User Service wasn't found!?");
            User user = userService.GetByName(name) ?? throw new Exception("User not found");
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(user.APIKey));
        }

        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            JwtSecurityToken incomingToken = ReadJwtToken(token);
            string userName = incomingToken.Claims.First(claim => claim.Type == "Name").Value;

            SecurityKey securityKey = GetSSKeyForName(userName);
            validationParameters.IssuerSigningKey = securityKey;
            return base.ValidateToken(token, validationParameters, out validatedToken);
        }
    }
}
