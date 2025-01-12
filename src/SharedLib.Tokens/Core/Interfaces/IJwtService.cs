using Microsoft.IdentityModel.Tokens;
using SharedLib.Tokens.Core.Models;
using System.Collections.ObjectModel;

namespace SharedLib.Tokens.Core.Interfaces;

public interface IJwtService
{
    Task<SecurityKey> GenerateKey();
    Task<SecurityKey> GetCurrentSecurityKey();
    Task<SigningCredentials> GetCurrentSigningCredentials();
    Task<EncryptingCredentials> GetCurrentEncryptingCredentials();
    Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int? i = null);
    Task RevokeKey(string keyId, string reason = null);
    Task<SecurityKey> GenerateNewKey();
}
[Obsolete("Deprecate, use IJwtServiceInstead")]
public interface IJsonWebKeySetService : IJwtService { }
