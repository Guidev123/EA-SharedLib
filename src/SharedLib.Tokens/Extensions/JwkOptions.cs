namespace SharedLib.Tokens.Extensions;

public class JwkOptions
{
    public JwkOptions() { }
    public JwkOptions(string jwksUri, string issuer = null, TimeSpan? cacheTime = null, string audience = null)
    {
        JwksUri = jwksUri;
        var jwks = new Uri(jwksUri);
        Issuer = issuer ?? $"{jwks.Scheme}://{jwks.Authority}";
        KeepFor = cacheTime ?? TimeSpan.FromMinutes(15);
        Audience = audience;
    }
    public string Issuer { get; set; } = string.Empty;
    public string JwksUri { get; set; } = string.Empty;
    public TimeSpan KeepFor { get; set; } = TimeSpan.FromMinutes(15);
    public string Audience { get; set; } = string.Empty;
}
