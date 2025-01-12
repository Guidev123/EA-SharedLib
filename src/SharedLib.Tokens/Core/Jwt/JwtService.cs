using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLib.Tokens.Core.Interfaces;
using SharedLib.Tokens.Core.Models;
using System.Collections.ObjectModel;

namespace SharedLib.Tokens.Core.Jwt;

internal class JwtService(IJsonWebKeyStore store, IOptions<JwtOptions> options) : IJwtService
{
    private readonly IJsonWebKeyStore _store = store;
    private readonly IOptions<JwtOptions> _options = options;

    public async Task<SecurityKey> GenerateKey()
    {
        var key = new CryptographicKey(_options.Value.Jws);

        var model = new KeyMaterial(key);
        await _store.Store(model);

        return model.GetSecurityKey();
    }

    public async Task<SecurityKey> GetCurrentSecurityKey()
    {
        var current = await _store.GetCurrent();

        if (NeedsUpdate(current))
        {
            await _store.Revoke(current);
            var newKey = await GenerateKey();
            return newKey;
        }

        if (!await CheckCompatibility(current))
            current = await _store.GetCurrent();

        return current;
    }
    public async Task<SigningCredentials> GetCurrentSigningCredentials()
    {
        var current = await GetCurrentSecurityKey();

        return new SigningCredentials(current, _options.Value.Jws);
    }

    public async Task<EncryptingCredentials> GetCurrentEncryptingCredentials()
    {
        var current = await GetCurrentSecurityKey();

        return new EncryptingCredentials(current, _options.Value.Jwe.Alg, _options.Value.Jwe.EncryptionAlgorithmContent);
    }

    public Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int? i = null)
    {
        return _store.GetLastKeys(_options.Value.AlgorithmsToKeep);
    }

    private async Task<bool> CheckCompatibility(KeyMaterial currentKey)
    {
        if (currentKey.Type != _options.Value.Jws.Kty())
        {
            await GenerateKey();
            return false;
        }
        return true;
    }

    public async Task RevokeKey(string keyId, string reason = null)
    {
        var key = await _store.Get(keyId);

        await _store.Revoke(key, reason);
    }

    public async Task<SecurityKey> GenerateNewKey()
    {
        var oldCurrent = await _store.GetCurrent();
        await _store.Revoke(oldCurrent);
        return await GenerateKey();

    }

    private bool NeedsUpdate(KeyMaterial current)
        => current == null || current.IsExpired(_options.Value.DaysUntilExpire) || current.IsRevoked;
}
