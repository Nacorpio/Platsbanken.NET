using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Platsbanken.Application;
using Platsbanken.Infrastructure;
using Platsbanken.Infrastructure.Configuration;

namespace Platsbanken;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Composition root: registers use cases (<see cref="JobSearchService"/>, <see cref="JobStreamService"/>)
    /// and the HTTP adapters behind the domain ports.
    /// </summary>
    public static IServiceCollection AddPlatsbanken(
        this IServiceCollection services,
        Action<PlatsbankenOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton(TimeProvider.System);
        services.AddPlatsbankenInfrastructure(configure);
        services.TryAddTransient<JobSearchService>();
        services.TryAddTransient<JobStreamService>();
        return services;
    }
}
