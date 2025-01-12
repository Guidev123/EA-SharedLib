using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedLib.Tokens.Core.Interfaces;

namespace SharedLib.Tokens.EntityFramework;

public static class EFCoreServiceExtensions
{
    public static IJwksBuilder PersistKeysToDatabaseStore<TContext>(this IJwksBuilder builder)
        where TContext : DbContext, ISecurityKeyContext
    {
        builder.Services.AddScoped<IJsonWebKeyStore, DatabaseJsonWebKeyStore<TContext>>();

        return builder;
    }
}
