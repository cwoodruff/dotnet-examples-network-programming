using System.Collections.Concurrent;
using backend_long_running_query.Models;

namespace backend_long_running_query.Services;

public class BackgroundJobQueue : IBackgroundJobQueue
{
    private readonly ConcurrentQueue<QueryJob> _jobs = new();
    private readonly SemaphoreSlim _signal = new(0);

    public void Enqueue(QueryJob job)
    {
        if (job == null)
        {
            throw new ArgumentNullException(nameof(job));
        }
        _jobs.Enqueue(job);
        _signal.Release();
    }

    public async Task<QueryJob> DequeueAsync(CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);
        _jobs.TryDequeue(out var job);
        return job;
    }
}
