using System.Security.Cryptography;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using SharedLib.Tokens.Core.Jwa;

namespace SharedLib.Tokens.Core.Services;

internal static class CryptoService
{
    public static RsaSecurityKey CreateRsaSecurityKey(int keySize = 3072)
    {
        return new RsaSecurityKey(RSA.Create(keySize))
        {
            KeyId = CreateUniqueId()
        };
    }

    internal static string CreateUniqueId(int length = 16)
    {
        return Base64UrlEncoder.Encode(CreateRandomKey(length));
    }

    internal static ECDsaSecurityKey CreateECDsaSecurityKey(Algorithm algorithm)
    {
        var curve = algorithm.Curve;
        if (string.IsNullOrEmpty(algorithm.Curve))
            curve = GetCurveType(algorithm);

        return new ECDsaSecurityKey(ECDsa.Create(GetNamedECCurve(curve)))
        {
            KeyId = CreateUniqueId()
        };
    }

    internal static string GetCurveType(Algorithm algorithm)
    {
        return algorithm.Alg switch
        {
            SecurityAlgorithms.EcdsaSha256 => JsonWebKeyECTypes.P256,
            SecurityAlgorithms.EcdsaSha384 => JsonWebKeyECTypes.P384,
            SecurityAlgorithms.EcdsaSha512 => JsonWebKeyECTypes.P521,
            _ => throw new InvalidOperationException($"Unsupported curve type for {algorithm}")
        };
    }

    internal static HMAC CreateHmacSecurityKey(Algorithm algorithm)
    {
        var hmac = algorithm.Alg switch
        {
            SecurityAlgorithms.HmacSha256 => (HMAC)new HMACSHA256(CreateRandomKey(64)),
            SecurityAlgorithms.HmacSha384 => new HMACSHA384(CreateRandomKey(128)),
            SecurityAlgorithms.HmacSha512 => new HMACSHA512(CreateRandomKey(128)),
            _ => throw new CryptographicException("Could not create HMAC key based on algorithm " + algorithm +
                                                  " (Could not parse expected SHA version)")
        };

        return hmac;
    }


    internal static Aes CreateAESSecurityKey(Algorithm algorithm)
    {
        var aesKey = Aes.Create();
        var aesKeySize = algorithm.Alg switch
        {
            SecurityAlgorithms.Aes128KW => 128,
            SecurityAlgorithms.Aes256KW => 256,
            _ => throw new CryptographicException("Could not create AES key based on algorithm " + algorithm)
        };
        aesKey.KeySize = aesKeySize;
        aesKey.GenerateKey();
        return aesKey;
    }

    internal static ECCurve GetNamedECCurve(string curveId)
    {
        if (string.IsNullOrEmpty(curveId))
            throw LogHelper.LogArgumentNullException(nameof(curveId));

        return curveId switch
        {
            JsonWebKeyECTypes.P256 => ECCurve.NamedCurves.nistP256,
            JsonWebKeyECTypes.P384 => ECCurve.NamedCurves.nistP384,
            JsonWebKeyECTypes.P512 => ECCurve.NamedCurves.nistP521,
            JsonWebKeyECTypes.P521 => ECCurve.NamedCurves.nistP521,
            _ => throw LogHelper.LogExceptionMessage(new ArgumentException(curveId))
        };
    }

    internal static byte[] CreateRandomKey(int length)
    {
        byte[] data = new byte[length];
        RandomNumberGenerator.Fill(data);
        return data;
    }
}
