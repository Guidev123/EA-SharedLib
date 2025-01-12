using Microsoft.Extensions.DependencyInjection;
using SharedLib.Tokens.Core.Interfaces;

namespace SharedLib.Tokens.Core;

public class JwksBuilder(IServiceCollection services) : IJwksBuilder
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));
}
