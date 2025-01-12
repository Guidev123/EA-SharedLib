using Microsoft.Extensions.DependencyInjection;
using SharedLib.Tokens.Core.Interfaces;

namespace SharedLib.Tokens.Core;

public class JwksBuilder : IJwksBuilder
{

    public JwksBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public IServiceCollection Services { get; }
}
