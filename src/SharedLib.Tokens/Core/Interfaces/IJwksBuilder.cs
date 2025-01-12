using Microsoft.Extensions.DependencyInjection;

namespace SharedLib.Tokens.Core.Interfaces;

public interface IJwksBuilder
{
    IServiceCollection Services { get; }
}
