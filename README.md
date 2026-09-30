# Platsbanken.NET

An unofficial .NET 10 client for the open job-ad APIs from Arbetsförmedlingen (the Swedish Public Employment Service), published through [JobTech Dev](https://jobtechdev.se).

> **Unofficial.** This project is not affiliated with, endorsed by, or supported by Arbetsförmedlingen or JobTech Dev. "Platsbanken" is the name of Arbetsförmedlingen's job portal and is used here only to describe what the library talks to.

## Features

- **JobSearch**: full-text and taxonomy-filtered search, get ad by id, automatic paging.
- **JobStream**: lazy `IAsyncEnumerable<JobAd>` over the JSON Lines snapshot and change stream. Memory stays flat, even for the full snapshot.
- **Taxonomy**: look up concept ids (municipality, region, occupation, skill and more) by label, cached in memory.
- No API key needed. The upstream APIs are open.
- Domain-driven layering with ports and adapters, dependency injection first, and resilience (retry, timeout, circuit breaker) built in.

## Install

```
dotnet add package Platsbanken.Client
```

## Use

```csharp
builder.Services.AddPlatsbanken(o => o.UserAgent = "MyApp/1.0 (me@example.com)");
```

```csharp
public sealed class Worker(JobSearchService search, JobStreamService stream)
{
    public async Task RunAsync(CancellationToken ct)
    {
        var page = await search.SearchAsync(new JobSearchCriteria { Query = "utvecklare", Limit = 20 }, ct);

        await foreach (var ad in stream.ChangesSinceAsync(TimeSpan.FromHours(1), ct))
        {
            Console.WriteLine($"{ad.Id} removed={ad.IsRemoved} {ad.Headline}");
        }
    }
}
```

Options bind from `PlatsbankenOptions` (base addresses, user agent, timeouts). Please set a descriptive `UserAgent`.

## Architecture

```
Platsbanken.Domain          model (JobAd, ConceptId, ...) and ports (IJobAdSearch, IJobAdStream). No dependencies.
Platsbanken.Application     use cases (JobSearchService, JobStreamService).
Platsbanken.Infrastructure  HTTP adapters, wire DTOs, mapping (anti-corruption layer), resilience.
Platsbanken.Client          composition root: services.AddPlatsbanken().
```

The dependency rule is enforced by architecture tests. Wire types are `internal`, so upstream API changes stay contained in the mapper.

## Data and terms

The data comes from Arbetsförmedlingen via JobTech Dev. Check the [JobTech terms and licences](https://jobtechdev.se) for the data you use, and respect the API rate limits.

## Build

```
dotnet build
dotnet test
dotnet run --project samples/Platsbanken.Sample   # live smoke test against the real APIs
```

## Roadmap

- JobAd Enrichments and Historical Ads

## License

[MIT](LICENSE)
