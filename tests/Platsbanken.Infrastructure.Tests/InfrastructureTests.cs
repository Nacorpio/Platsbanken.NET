using System.Net;
using System.Text;
using Platsbanken.Domain.Ads;
using Platsbanken.Domain.Search;
using Platsbanken.Domain.Streaming;
using Platsbanken.Infrastructure.Http;
using Platsbanken.Infrastructure.Mapping;
using Platsbanken.Infrastructure.Wire;
using System.Text.Json;

namespace Platsbanken.Infrastructure.Tests;

public class InfrastructureTests
{
    private const string AdJson = """
        {"id":"123","webpage_url":"https://arbetsformedlingen.se/platsbanken/annonser/123","headline":"Utvecklare",
         "application_deadline":"2026-10-22T23:59:59","description":{"text":"Bra jobb"},
         "employer":{"name":"Acme AB","organization_number":"5561234567"},
         "occupation":{"concept_id":"abc","label":"Systemutvecklare"},
         "workplace_address":{"municipality":"Stockholm","municipality_concept_id":"AvNB_uwa_6n6","coordinates":[18.07,59.33]},
         "driving_license_required":true,"removed":false}
        """;

    [Fact]
    public void Mapper_maps_wire_ad_to_domain()
    {
        var wire = JsonSerializer.Deserialize(AdJson, WireJsonContext.Default.WireJobAd);
        var ad = JobAdMapper.ToDomain(wire);

        Assert.NotNull(ad);
        Assert.Equal("123", ad.Id.Value);
        Assert.Equal("Systemutvecklare", ad.Occupation?.Label);
        Assert.Equal("Acme AB", ad.Employer?.Name);
        Assert.Equal(new GeoPoint(18.07, 59.33), ad.Workplace?.Coordinates);
        Assert.True(ad.DrivingLicenseRequired);
        Assert.Equal(TimeSpan.FromHours(2), ad.ApplicationDeadline?.Offset); // CEST in October
    }

    [Fact]
    public void Mapper_tolerates_null_coordinates()
    {
        var json = "{\"id\":\"9\",\"workplace_address\":{\"city\":\"X\",\"coordinates\":[null,null]}}";
        var ad = JobAdMapper.ToDomain(JsonSerializer.Deserialize(json, WireJsonContext.Default.WireJobAd));

        Assert.NotNull(ad?.Workplace);
        Assert.Null(ad.Workplace.Coordinates);
    }

    [Fact]
    public void Mapper_returns_null_without_id()
        => Assert.Null(JobAdMapper.ToDomain(new WireJobAd { Headline = "x" }));

    [Fact]
    public void SearchUri_repeats_multi_value_params_and_escapes()
    {
        var uri = JobSearchClient.BuildSearchUri(new JobSearchCriteria
        {
            Query = "c# utvecklare",
            Municipalities = [new ConceptId("a"), new ConceptId("b")],
            Limit = 25,
        });

        Assert.Equal("search?q=c%23%20utvecklare&municipality=a&municipality=b&offset=0&limit=25", uri);
    }

    [Fact]
    public void StreamUri_uses_local_swedish_time()
    {
        var window = new StreamWindow(new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero));
        Assert.Equal("v2/stream?updated-after=2026-01-15T11%3A00%3A00", JobStreamClient.BuildStreamUri(window));
    }

    [Fact]
    public async Task StreamClient_reads_json_lines_lazily_and_skips_blank_lines()
    {
        var body = string.Join('\n', """{"id":"1","headline":"A"}""", "", """{"id":"2","removed":true}""");
        using var http = new HttpClient(new StubHandler(body)) { BaseAddress = new Uri("https://example.test/") };
        var client = new JobStreamClient(http);

        var ads = new List<JobAd>();
        await foreach (var ad in client.SnapshotAsync())
        {
            ads.Add(ad);
        }

        Assert.Equal(["1", "2"], ads.Select(a => a.Id.Value));
        Assert.True(ads[1].IsRemoved);
    }

    private sealed class StubHandler(string body) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/jsonl"),
            });
    }
}
