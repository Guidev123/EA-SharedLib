using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedLib.Tokens.Core.Interfaces;

namespace SharedLib.Tokens.AspNet;

public static class AspNetBuilderExtensions
{
    public static IApplicationBuilder UseJwksDiscovery(this IApplicationBuilder app, string jwtDiscoveryEndpoint = "/jwks")
    {
        if (!jwtDiscoveryEndpoint.StartsWith('/')) throw new ArgumentException("The Jwks URI must starts with '/'");

        app.Map(new PathString(jwtDiscoveryEndpoint), x =>
            x.UseMiddleware<JwtServiceDiscoveryMiddleware>());

        return app;
    }

    public static IJwksBuilder UseJwtValidation(this IJwksBuilder builder)
    {

        builder.Services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>>(s => new JwtPostConfigureOptions(s));

        return builder;
    }
}
