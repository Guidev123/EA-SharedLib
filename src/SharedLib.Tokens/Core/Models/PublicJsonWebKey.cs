using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

namespace SharedLib.Tokens.Core.Models;

public class PublicJsonWebKey
{
    public PublicJsonWebKey(JsonWebKey jsonWebKey)
    {
        KeyType = jsonWebKey.Kty;
        PublicKeyUse = jsonWebKey.Use ?? "sig";
        KeyId = jsonWebKey.Kid;
        Algorithm = jsonWebKey.Alg;
        if (jsonWebKey.KeyOps.Any())
            KeyOperations = jsonWebKey.KeyOps;
        if (jsonWebKey.X5c.Any())
            X509Chain = jsonWebKey.X5c;
        X509Url = jsonWebKey.X5u;
        X5tS256 = jsonWebKey.X5t;

        if (jsonWebKey.Kty == JsonWebAlgorithmsKeyTypes.EllipticCurve)
        {
            CurveName = jsonWebKey.Crv;
            X = jsonWebKey.X;
            Y = jsonWebKey.Y;
        }

        if (jsonWebKey.Kty == JsonWebAlgorithmsKeyTypes.RSA)
        {
            Modulus = jsonWebKey.N;
            Exponent = jsonWebKey.E;
        }

        if (jsonWebKey.Kty == JsonWebAlgorithmsKeyTypes.Octet)
        {
            Key = jsonWebKey.K;
        }
    }

    [JsonPropertyName("kty")]
    public string KeyType { get; }

    [JsonPropertyName("use")]
    public string PublicKeyUse { get; private set; }

    [JsonPropertyName("key_ops")]
    public IList<string> KeyOperations { get; }

    [JsonPropertyName("alg")]
    public string Algorithm { get; }

    [JsonPropertyName("kid")]
    public string KeyId { get; }

    [JsonPropertyName("x5u")]
    public string X509Url { get; set; }

    [JsonPropertyName("x5c")]
    public IList<string> X509Chain { get; set; }

    [JsonPropertyName("x5t")]
    public string X5tS256 { get; set; }

    [JsonPropertyName("crv")]
    public string CurveName { get; }

    [JsonPropertyName("x")]
    public string X { get; }

    [JsonPropertyName("y")]
    public string Y { get; }


    [JsonPropertyName("n")]
    public string Modulus { get; set; }

    [JsonPropertyName("e")]
    public string Exponent { get; set; }


    [JsonPropertyName("k")]
    public string Key { get; set; }


    public static PublicJsonWebKey FromJwk(JsonWebKey jwk)
    {
        return new PublicJsonWebKey(jwk);
    }

    public JsonWebKey ToNativeJwk()
    {
        var jsonWebKey = new JsonWebKey
        {
            Kty = KeyType,
            Use = PublicKeyUse,
            Kid = KeyId,
            Alg = Algorithm,
            X5u = X509Url,
            X5t = X5tS256,
            Crv = CurveName,
            X = X,
            Y = Y,
            N = Modulus,
            E = Exponent,
            K = Key
        };

        if (KeyOperations != null)
            foreach (var keyOperation in KeyOperations)
                jsonWebKey.KeyOps.Add(keyOperation);

        if (X509Chain != null)
            foreach (var certificate in X509Chain)
                jsonWebKey.X5c.Add(certificate);

        return jsonWebKey;
    }
}
