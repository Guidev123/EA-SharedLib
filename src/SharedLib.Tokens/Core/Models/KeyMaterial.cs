using Microsoft.IdentityModel.Tokens;
using SharedLib.Tokens.Core.Jwa;
using SharedLib.Tokens.Core.Services;

namespace SharedLib.Tokens.Core.Models;

public class CryptographicKey
{
    public CryptographicKey(Algorithm algorithm)
    {
        Algorithm = algorithm;
        Key = algorithm.AlgorithmType switch
        {
            AlgorithmType.RSA => GenerateRsa(),
            AlgorithmType.ECDsa => GenerateECDsa(algorithm),
            AlgorithmType.HMAC => GenerateHMAC(algorithm),
            AlgorithmType.AES => GenerateAES(algorithm),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null)
        };
    }

    public Algorithm Algorithm { get; set; }
    public SecurityKey Key { get; set; }

    public JsonWebKey GetJsonWebKey() => Algorithm.AlgorithmType switch
    {
        AlgorithmType.RSA => JsonWebKeyConverter.ConvertFromRSASecurityKey((RsaSecurityKey)Key),
        AlgorithmType.ECDsa => JsonWebKeyConverter.ConvertFromECDsaSecurityKey((ECDsaSecurityKey)Key),
        AlgorithmType.HMAC => JsonWebKeyConverter.ConvertFromSymmetricSecurityKey((SymmetricSecurityKey)Key),
        AlgorithmType.AES => JsonWebKeyConverter.ConvertFromSymmetricSecurityKey((SymmetricSecurityKey)Key),
        _ => throw new ArgumentOutOfRangeException()
    };

    private SecurityKey GenerateRsa() => CryptoService.CreateRsaSecurityKey();
    private SecurityKey GenerateECDsa(Algorithm algorithm) => CryptoService.CreateECDsaSecurityKey(algorithm);
    private SecurityKey GenerateHMAC(Algorithm jwsAlgorithms)
    {
        var key = CryptoService.CreateHmacSecurityKey(jwsAlgorithms);
        return new SymmetricSecurityKey(key.Key)
        {
            KeyId = CryptoService.CreateUniqueId()
        };
    }

    private SecurityKey GenerateAES(Algorithm jwsAlgorithms)
    {
        var key = CryptoService.CreateAESSecurityKey(jwsAlgorithms);
        return new SymmetricSecurityKey(key.Key)
        {
            KeyId = CryptoService.CreateUniqueId()
        };
    }

    public static implicit operator SecurityKey(CryptographicKey value) => value.Key;
}
