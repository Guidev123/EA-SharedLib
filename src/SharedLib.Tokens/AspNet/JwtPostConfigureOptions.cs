using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace SharedLib.Tokens.AspNet;

public class JwtPostConfigureOptions(IServiceProvider serviceProvider) : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.TokenHandlers.Clear();
        options.TokenHandlers.Add(new JwtServiceValidationHandler(_serviceProvider));
        options.SecurityTokenValidators.Clear();
        options.SecurityTokenValidators.Add(new JwtServiceValidationHandler(_serviceProvider));
    }
}