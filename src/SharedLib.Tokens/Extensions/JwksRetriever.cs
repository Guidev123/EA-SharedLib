using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace SharedLib.Tokens.Extensions;

public class JwksRetriever : IConfigurationRetriever<OpenIdConnectConfiguration>
{
    public Task<OpenIdConnectConfiguration> GetConfigurationAsync(string address, IDocumentRetriever retriever,
        CancellationToken cancel) => GetAsync(address, retriever, cancel);

    public static async Task<OpenIdConnectConfiguration> GetAsync(string address, IDocumentRetriever retriever, CancellationToken cancel)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw LogHelper.LogArgumentNullException(nameof(address));

        if (retriever == null)
            throw LogHelper.LogArgumentNullException(nameof(retriever));

        IdentityModelEventSource.ShowPII = true;
        var doc = await retriever.GetDocumentAsync(address, cancel);
        LogHelper.LogVerbose("IDX21811: Deserializing the string: '{0}' obtained from metadata endpoint into openIdConnectConfiguration object.", doc);
        var jwks = new JsonWebKeySet(doc);
        var openIdConnectConfiguration = new OpenIdConnectConfiguration()
        {
            JsonWebKeySet = jwks,
            JwksUri = address,
        };
        foreach (var securityKey in jwks.GetSigningKeys())
            openIdConnectConfiguration.SigningKeys.Add(securityKey);

        return openIdConnectConfiguration;
    }
}