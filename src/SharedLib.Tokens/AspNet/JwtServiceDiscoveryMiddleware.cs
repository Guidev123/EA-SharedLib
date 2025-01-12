using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SharedLib.Tokens.Core;
using SharedLib.Tokens.Core.Interfaces;
using SharedLib.Tokens.Core.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharedLib.Tokens.AspNet;

public class JwtServiceDiscoveryMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext httpContext, IJwtService keyService, IOptions<JwtOptions> options)
    {
        var storedKeys = await keyService.GetLastKeys(options.Value.AlgorithmsToKeep);
        var keys = new
        {
            keys = storedKeys.Select(s => s.GetSecurityKey()).Select(PublicJsonWebKey.FromJwk)
        };
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(keys, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }));
    }
}