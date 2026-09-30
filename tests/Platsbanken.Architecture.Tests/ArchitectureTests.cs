using NetArchTest.Rules;
using Platsbanken.Domain.Ads;

namespace Platsbanken.Architecture.Tests;

public class ArchitectureTests
{
    private static readonly System.Reflection.Assembly Domain = typeof(JobAd).Assembly;
    private static readonly System.Reflection.Assembly Application = typeof(Platsbanken.Application.JobSearchService).Assembly;
    private static readonly System.Reflection.Assembly Infrastructure = typeof(Platsbanken.Infrastructure.ServiceCollectionExtensions).Assembly;

    [Fact]
    public void Domain_depends_on_nothing_else_in_the_solution()
    {
        var result = Types.InAssembly(Domain)
            .ShouldNot().HaveDependencyOnAny("Platsbanken.Application", "Platsbanken.Infrastructure", "Platsbanken.Client")
            .GetResult();
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Domain_has_no_http_or_di_dependencies()
    {
        var result = Types.InAssembly(Domain)
            .ShouldNot().HaveDependencyOnAny("System.Net.Http", "Microsoft.Extensions")
            .GetResult();
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Application_does_not_depend_on_infrastructure()
    {
        var result = Types.InAssembly(Application)
            .ShouldNot().HaveDependencyOnAny("Platsbanken.Infrastructure", "Platsbanken.Client", "System.Net.Http")
            .GetResult();
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Wire_types_and_adapters_stay_internal()
    {
        var result = Types.InAssembly(Infrastructure)
            .That().ResideInNamespaceStartingWith("Platsbanken.Infrastructure.Wire")
            .Or().ResideInNamespaceStartingWith("Platsbanken.Infrastructure.Http")
            .Or().ResideInNamespaceStartingWith("Platsbanken.Infrastructure.Mapping")
            .Should().NotBePublic()
            .GetResult();
        Assert.True(result.IsSuccessful);
    }
}
