using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Platsbanken.Domain.Search;
using Platsbanken.Domain.Streaming;
using Platsbanken.Infrastructure.Configuration;
using Platsbanken.Infrastructure.Http;

namespace Platsbanken.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>Registers the HTTP adapters for the JobSearch and JobStream ports.</summary>
    public static IServiceCollection AddPlatsbankenInfrastructure(
        this IServiceCollection services,
        Action<PlatsbankenOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<PlatsbankenOptions>().ValidateDataAnnotations().ValidateOnStart();
        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.AddHttpClient<IJobAdSearch, JobSearchClient>((sp, client) =>
            {
                var o = sp.GetRequiredService<IOptions<PlatsbankenOptions>>().Value;
                client.BaseAddress = o.SearchBaseAddress;
                client.Timeout = Timeout.InfiniteTimeSpan;
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(o.UserAgent, null));
            })
            .AddStandardResilienceHandler()
            .Configure((options, sp) =>
            {
                var o = sp.GetRequiredService<IOptions<PlatsbankenOptions>>().Value;
                options.AttemptTimeout.Timeout = o.SearchTimeout;
                options.TotalRequestTimeout.Timeout = o.SearchTimeout * 4;
                options.CircuitBreaker.SamplingDuration = o.SearchTimeout * 2;
            });

        services.AddHttpClient<IJobAdStream, JobStreamClient>((sp, client) =>
            {
                var o = sp.GetRequiredService<IOptions<PlatsbankenOptions>>().Value;
                client.BaseAddress = o.StreamBaseAddress;
                client.Timeout = Timeout.InfiniteTimeSpan;
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(o.UserAgent, null));
            })
            .AddStandardResilienceHandler()
            .Configure((options, sp) =>
            {
                var o = sp.GetRequiredService<IOptions<PlatsbankenOptions>>().Value;
                options.AttemptTimeout.Timeout = o.StreamTimeout;
                options.TotalRequestTimeout.Timeout = o.StreamTimeout * 2;
                options.CircuitBreaker.SamplingDuration = o.StreamTimeout * 2;
            });

        return services;
    }
}
