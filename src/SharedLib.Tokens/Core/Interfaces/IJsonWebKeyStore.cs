using SharedLib.Tokens.Core.Models;
using System.Collections.ObjectModel;

namespace SharedLib.Tokens.Core.Interfaces;

public interface IJsonWebKeyStore
{
    Task Store(KeyMaterial keyMaterial);
    Task<KeyMaterial> GetCurrent();
    Task Revoke(KeyMaterial keyMaterial, string reason = default);
    Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int quantity);
    Task<KeyMaterial> Get(string keyId);
    Task Clear();
}
