using Platsbanken.Domain.Ads;
using Platsbanken.Domain.Streaming;

namespace Platsbanken.Application;

/// <summary>Use cases over <see cref="IJobAdStream"/>.</summary>
public sealed class JobStreamService(IJobAdStream stream, TimeProvider clock)
{
    public IAsyncEnumerable<JobAd> SnapshotAsync(CancellationToken cancellationToken = default)
        => stream.SnapshotAsync(cancellationToken);

    public IAsyncEnumerable<JobAd> StreamAsync(StreamWindow window, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(window);
        return stream.StreamAsync(window, cancellationToken);
    }

    /// <summary>Convenience: changes over the last span of time.</summary>
    public IAsyncEnumerable<JobAd> ChangesSinceAsync(TimeSpan span, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(span, TimeSpan.Zero);
        return stream.StreamAsync(new StreamWindow(clock.GetUtcNow() - span), cancellationToken);
    }
}
