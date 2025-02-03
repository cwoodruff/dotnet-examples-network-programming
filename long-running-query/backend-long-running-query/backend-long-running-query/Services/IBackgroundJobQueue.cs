using backend_long_running_query.Models;

namespace backend_long_running_query.Services;

public interface IBackgroundJobQueue
{
    void Enqueue(QueryJob job);
    Task<QueryJob> DequeueAsync(CancellationToken cancellationToken);
}