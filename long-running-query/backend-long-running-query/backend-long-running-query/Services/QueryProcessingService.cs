using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using backend_long_running_query.Data;
using backend_long_running_query.Hubs;
using backend_long_running_query.Services;

namespace LongRunningQueryAPI.Services
{
    public class QueryProcessingService : BackgroundService
    {
        private readonly IBackgroundJobQueue _jobQueue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<QueryProcessingService> _logger;
        private readonly IHubContext<QueryHub> _hubContext;

        public QueryProcessingService(IBackgroundJobQueue jobQueue,
                                      IServiceScopeFactory scopeFactory,
                                      ILogger<QueryProcessingService> logger,
                                      IHubContext<QueryHub> hubContext)
        {
            _jobQueue = jobQueue;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Wait for the next job.
                var job = await _jobQueue.DequeueAsync(stoppingToken);
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

                    // Execute the long-running query.
                    var result = await dbContext.MyEntities
                        .Where(e => e.SomeProperty == job.QueryParameter)
                        .ToListAsync(stoppingToken);

                    // Option 1: If a callback URL is provided, send the result via HTTP POST.
                    if (!string.IsNullOrWhiteSpace(job.CallbackUrl))
                    {
                        using var client = new HttpClient();
                        await client.PostAsJsonAsync(job.CallbackUrl,
                            new { jobId = job.Id, result }, cancellationToken: stoppingToken);
                    }
                    else
                    {
                        // Option 2: Otherwise, notify the client using SignalR.
                        await _hubContext.Clients.Group(job.Id.ToString())
                            .SendAsync("QueryCompleted", job.Id.ToString(), result, cancellationToken: stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing job {JobId}", job.Id);

                    // Notify the client about the error.
                    await _hubContext.Clients.Group(job.Id.ToString())
                        .SendAsync("QueryFailed", job.Id.ToString(), ex.Message, cancellationToken: stoppingToken);
                }
            }
        }
    }
}
