using Microsoft.IdentityModel.Tokens;

namespace SharedLib.Tokens.Extensions;

public sealed class JwkList(JsonWebKeySet jwkTaskResult)
{
    public DateTime When { get; set; } = DateTime.Now;
    public JsonWebKeySet Jwks { get; set; } = jwkTaskResult;
}
