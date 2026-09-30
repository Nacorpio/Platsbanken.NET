using Platsbanken.Domain.Ads;

namespace Platsbanken.Domain.Streaming;

/// <summary>Port: bulk access to all published ads and their changes.</summary>
public interface IJobAdStream
{
    /// <summary>All currently published ads.</summary>
    IAsyncEnumerable<JobAd> SnapshotAsync(CancellationToken cancellationToken = default);

    /// <summary>Ads created, changed or removed inside the window. Removed ads have IsRemoved set.</summary>
    IAsyncEnumerable<JobAd> StreamAsync(StreamWindow window, CancellationToken cancellationToken = default);
}
