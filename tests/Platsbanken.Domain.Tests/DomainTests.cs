using Platsbanken.Domain.Ads;
using Platsbanken.Domain.Search;
using Platsbanken.Domain.Streaming;

namespace Platsbanken.Domain.Tests;

public class DomainTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void ConceptId_rejects_blank(string? value)
        => Assert.ThrowsAny<ArgumentException>(() => new ConceptId(value!));

    [Fact]
    public void StreamWindow_rejects_inverted_range()
    {
        var now = DateTimeOffset.UtcNow;
        Assert.Throws<ArgumentException>(() => new StreamWindow(now, now.AddMinutes(-1)));
    }

    [Theory]
    [InlineData(0, 1, true)]
    [InlineData(2000, 100, true)]
    [InlineData(2001, 10, false)]
    [InlineData(-1, 10, false)]
    [InlineData(0, 101, false)]
    [InlineData(0, 0, false)]
    public void SearchCriteria_validates_paging(int offset, int limit, bool valid)
    {
        var criteria = new JobSearchCriteria { Offset = offset, Limit = limit };
        if (valid)
        {
            criteria.Validate();
        }
        else
        {
            Assert.Throws<ArgumentOutOfRangeException>(criteria.Validate);
        }
    }
}
