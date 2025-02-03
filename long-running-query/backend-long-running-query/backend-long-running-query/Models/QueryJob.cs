namespace backend_long_running_query.Models;

public class QueryJob
{
    public Guid Id { get; set; }
    // Add any parameters needed to run your query:
    public string QueryParameter { get; set; }
    // Optionally include a callback URL if you wish to use webhooks:
    public string? CallbackUrl { get; set; }
}