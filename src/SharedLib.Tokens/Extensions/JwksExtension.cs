using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;

namespace SharedLib.Tokens.Extensions;

public static class JwksExtension
{
    public static void SetJwksOptions(this JwtBearerOptions options, JwkOptions jwkOptions)
    {
        var httpClient = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler())
        {
            Timeout = options.BackchannelTimeout,
            MaxResponseContentBufferSize = 1024 * 1024 * 10 
        };

        options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            jwkOptions.JwksUri,
            new JwksRetriever(),
            new HttpDocumentRetriever(httpClient) { RequireHttps = options.RequireHttpsMetadata });

        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.ValidIssuer = jwkOptions.Issuer;

        if (!string.IsNullOrEmpty(jwkOptions.Audience))
        {
            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidAudience = jwkOptions.Audience;
        }
    }
}
