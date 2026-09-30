using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Platsbanken;
using Platsbanken.Application;
using Platsbanken.Domain.Search;
using Platsbanken.Domain.Taxonomy;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddPlatsbanken(o => o.UserAgent = "Platsbanken.Sample");
using var host = builder.Build();

var search = host.Services.GetRequiredService<JobSearchService>();
var stream = host.Services.GetRequiredService<JobStreamService>();

var taxonomy = host.Services.GetRequiredService<TaxonomyService>();
var gothenburg = await taxonomy.FindByLabelAsync(ConceptType.Municipality, "Göteborg");
Console.WriteLine($"Taxonomy: Göteborg = {gothenburg?.Id}");

var result = await search.SearchAsync(new JobSearchCriteria
{
    Query = "utvecklare",
    Municipalities = gothenburg is null ? [] : [gothenburg.Id],
    Limit = 3,
});
Console.WriteLine($"Search: {result.Total} hits");
foreach (var ad in result.Hits)
{
    Console.WriteLine($"  {ad.Id} | {ad.Headline} | {ad.Employer?.Name} | {ad.Workplace?.Municipality?.Label}");
}

var fetched = await search.GetAsync(result.Hits[0].Id);
Console.WriteLine($"Get: {fetched?.Headline}");

var count = 0;
await foreach (var ad in stream.ChangesSinceAsync(TimeSpan.FromMinutes(30)))
{
    count++;
    if (count <= 3)
    {
        Console.WriteLine($"  stream: {ad.Id} removed={ad.IsRemoved} {ad.Headline}");
    }
}

Console.WriteLine($"Stream: {count} changes in the last 30 minutes");
