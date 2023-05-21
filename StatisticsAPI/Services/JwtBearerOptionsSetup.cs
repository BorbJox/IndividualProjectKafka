using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace StatisticsAPI.Services
{
    public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly DynamicJwtValidationHandler _validationHandler;


        public JwtBearerOptionsSetup(DynamicJwtValidationHandler validationHandler)
        {
            _validationHandler = validationHandler;
        }

        public void Configure(string? name, JwtBearerOptions options)
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
            };
            options.SecurityTokenValidators.Clear();
            options.SecurityTokenValidators.Add(_validationHandler);
        }

        public void Configure(JwtBearerOptions options) => Configure(Options.DefaultName, options);

    }
}
