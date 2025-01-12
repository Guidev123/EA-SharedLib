using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using SharedLib.Tokens.Core.Interfaces;
using SharedLib.Tokens.Core.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml.Linq;

namespace SharedLib.Tokens.Core.DefaultStore
{
    internal class DataProtectionStore : IJsonWebKeyStore
    {
        internal static readonly XName IdAttributeName = "id";
        internal static readonly XName VersionAttributeName = "version";
        internal static readonly XName CreationDateElementName = "creationDate";
        internal static readonly XName ActivationDateElementName = "activationDate";
        internal static readonly XName ExpirationDateElementName = "expirationDate";
        internal static readonly XName DescriptorElementName = "descriptor";
        internal static readonly XName DeserializerTypeAttributeName = "deserializerType";
        internal static readonly XName RevocationElementName = "NetDevPackSecurityJwtRevocation";
        internal static readonly XName RevocationDateElementName = "revocationDate";
        internal static readonly XName ReasonElementName = "reason";

        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptions<JwtOptions> _options;
        private readonly IOptions<KeyManagementOptions> _keyManagementOptions;
        private readonly IMemoryCache _memoryCache;
        private readonly IDataProtector _dataProtector;
        private IXmlRepository KeyRepository => _keyManagementOptions.Value.XmlRepository ?? GetFallbackKeyRepositoryEncryptorPair();

        private const string Name = "NetDevPackSecurityJwt";
        internal const string DefaultRevocationReason = "Revoked";

        public DataProtectionStore(
            ILoggerFactory loggerFactory,
            IOptions<JwtOptions> options,
            IDataProtectionProvider provider,
            IOptions<KeyManagementOptions> keyManagementOptions,
            IMemoryCache memoryCache)
        {
            _loggerFactory = loggerFactory;
            _options = options;
            _keyManagementOptions = keyManagementOptions;
            _memoryCache = memoryCache;
            _dataProtector = provider.CreateProtector(nameof(KeyMaterial)); ;
        }
        public Task Store(KeyMaterial securityParamteres)
        {
            var possiblyEncryptedKeyElement = _dataProtector.Protect(JsonSerializer.Serialize(securityParamteres));

            var keyElement = new XElement(Name,
                new XAttribute(IdAttributeName, securityParamteres.Id),
                new XAttribute(VersionAttributeName, 1),
                new XElement(CreationDateElementName, DateTimeOffset.UtcNow),
                new XElement(ActivationDateElementName, DateTimeOffset.UtcNow),
                new XElement(ExpirationDateElementName, DateTimeOffset.UtcNow.AddDays(_options.Value.DaysUntilExpire)),
                new XElement(DescriptorElementName,
                    new XAttribute(DeserializerTypeAttributeName, typeof(KeyMaterial).AssemblyQualifiedName!),
                    possiblyEncryptedKeyElement));

            var friendlyName = string.Format(CultureInfo.InvariantCulture, "key-{0}", securityParamteres.KeyId);
            KeyRepository.StoreElement(keyElement, friendlyName);
            ClearCache();

            return Task.CompletedTask;
        }



        public async Task<KeyMaterial> GetCurrent()
        {
            if (!_memoryCache.TryGetValue(JwkContants.CurrentJwkCache, out KeyMaterial keyMaterial))
            {
                var keys = await GetLastKeys(1);
                keyMaterial = keys.FirstOrDefault();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(_options.Value.CacheTime);

                if (keyMaterial != null)
                    _memoryCache.Set(JwkContants.CurrentJwkCache, keyMaterial, cacheEntryOptions);
            }

            return keyMaterial;
        }

        private IReadOnlyCollection<KeyMaterial> GetKeys()
        {
            var allElements = KeyRepository.GetAllElements();
            var keys = new List<KeyMaterial>();
            var revokedKeys = new List<RevokedKeyInfo>();
            foreach (var element in allElements)
            {
                if (element.Name == Name)
                {
                    var descriptorElement = element.Element(DescriptorElementName);
                    var expecteddescriptorType = typeof(KeyMaterial).FullName;
                    var descriptorType = descriptorElement.Attribute(DeserializerTypeAttributeName);

                    if (descriptorType == null || !descriptorType.Value.Contains(expecteddescriptorType))
                        continue;
                    string unencryptedInputToDeserializer = null;
                    try
                    {
                        unencryptedInputToDeserializer = _dataProtector.Unprotect(descriptorElement.Value);
                    }
                    catch
                    {
                        continue;
                    }
                    var key = JsonSerializer.Deserialize<KeyMaterial>(unencryptedInputToDeserializer);
                    if (key.IsExpired(_options.Value.DaysUntilExpire))
                        revokedKeys.Add(new RevokedKeyInfo(key.Id.ToString()));

                    keys.Add(key);
                }
                else if (element.Name == RevocationElementName)
                {
                    var keyIdAsString = (string)element.Element(Name)!.Attribute(IdAttributeName)!;
                    var reason = (string)element.Element(ReasonElementName);
                    revokedKeys.Add(new RevokedKeyInfo(keyIdAsString, reason));
                }
            }

            foreach (var revokedKey in revokedKeys)
            {
                keys.FirstOrDefault(a => a.Id.ToString().Equals(revokedKey.Id))?.Revoke(revokedKey.RevokedReason);
            }
            return keys.ToList();
        }


        public Task<ReadOnlyCollection<KeyMaterial>> GetLastKeys(int quantity = 5)
        {

            if (!_memoryCache.TryGetValue(JwkContants.JwksCache, out IReadOnlyCollection<KeyMaterial> keys))
            {
                keys = GetKeys();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(_options.Value.CacheTime);

                if (keys.Any())
                    _memoryCache.Set(JwkContants.JwksCache, keys, cacheEntryOptions);
            }

            return Task.FromResult(keys
                .OrderByDescending(s => s.CreationDate)
                .ToList()
                .AsReadOnly());
        }

        public Task<KeyMaterial> Get(string keyId)
        {
            var keys = GetKeys();
            return Task.FromResult(keys.FirstOrDefault(f => f.KeyId == keyId));
        }

        public async Task Clear()
        {
            foreach (var securityKeyWithPrivate in GetKeys())
            {
                await Revoke(securityKeyWithPrivate);
            }
        }


        public async Task Revoke(KeyMaterial keyMaterial, string reason = null)
        {
            if (keyMaterial == null)
                return;

            var keys = await GetLastKeys();
            var key = keys.First(f => f.Id == keyMaterial.Id);

            if (key is { IsRevoked: true })
                return;

            keyMaterial.Revoke();
            var revokeReason = reason ?? DefaultRevocationReason;
            var revocationElement = new XElement(RevocationElementName,
                new XAttribute(VersionAttributeName, 1),
                new XElement(RevocationDateElementName, DateTimeOffset.UtcNow),
                new XElement(Name,
                    new XAttribute(IdAttributeName, keyMaterial.Id)),
                new XElement(ReasonElementName, revokeReason));


            var friendlyName = string.Format(CultureInfo.InvariantCulture, "revocation-{0}-{1:D}-{2:yyyy_MM_dd_hh_mm_fffffff}", keyMaterial.Type, keyMaterial.Id, DateTime.UtcNow);
            KeyRepository.StoreElement(revocationElement, friendlyName);
            ClearCache();
        }


        private void ClearCache()
        {
            _memoryCache.Remove(JwkContants.JwksCache);
            _memoryCache.Remove(JwkContants.CurrentJwkCache);
        }

        internal IXmlRepository GetFallbackKeyRepositoryEncryptorPair()
        {
            IXmlRepository key;
            var forAzureWebSites = DefaultKeyStorageDirectories.Instance.GetKeyStorageDirectoryForAzureWebSites();
            if (forAzureWebSites != null)
            {
                key = new FileSystemXmlRepository(forAzureWebSites, this._loggerFactory);
            }
            else
            {
                var storageDirectory = DefaultKeyStorageDirectories.Instance.GetKeyStorageDirectory();
                if (storageDirectory != null)
                {
                    key = new FileSystemXmlRepository(storageDirectory, this._loggerFactory);
                }
                else
                {
                    RegistryKey registryKey = null;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        registryKey = RegistryXmlRepository.DefaultRegistryKey;

                    if (registryKey != null)
                    {
                        var defaultRegistryKey = RegistryXmlRepository.DefaultRegistryKey;
                        key = new RegistryXmlRepository(defaultRegistryKey, this._loggerFactory);
                    }
                    else
                    {
                        throw new Exception(
                            "Is not possible to determine which folder are the protection keys. NetDevPack.Security.JwtSigningCredentials.Store.FileSystem or NetDevPack.Security.JwtSigningCredentials.Store.EntityFrameworkCore");
                    }
                }
            }
            return key;
        }


    }
}
