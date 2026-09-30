using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Platsbanken.Application;
using Platsbanken.Infrastructure;
using Platsbanken.Infrastructure.Configuration;

namespace Platsbanken;

/// <summary>Entry point for consumers: <c>services.AddPlatsbanken()</c>.</summary>
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
        services.TryAddTransient<TaxonomyService>();
        return services;
    }
}
