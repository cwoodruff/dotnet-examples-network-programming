using backend_long_running_query.Models;
using backend_long_running_query.Services;

namespace backend_long_running_query.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class QueryController : ControllerBase
{
    private readonly IBackgroundJobQueue _jobQueue;

    public QueryController(IBackgroundJobQueue jobQueue)
    {
        _jobQueue = jobQueue;
    }

    public class StartQueryRequest
    {
        public string QueryParameter { get; set; } = string.Empty;
        // Optionally, include a callback URL for webhook notifications:
        public string? CallbackUrl { get; set; }
    }

    [HttpPost("start")]
    public IActionResult StartQuery([FromBody] StartQueryRequest request)
    {
        // Create a new job with a unique ID.
        var job = new QueryJob
        {
            Id = Guid.NewGuid(),
            QueryParameter = request.QueryParameter,
            CallbackUrl = request.CallbackUrl
        };

        // Enqueue the job.
        _jobQueue.Enqueue(job);

        // Return 202 Accepted with the job ID.
        return Accepted(new { jobId = job.Id });
    }
}