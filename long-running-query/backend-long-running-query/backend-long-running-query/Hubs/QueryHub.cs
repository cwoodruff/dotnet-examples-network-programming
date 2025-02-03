using Microsoft.AspNetCore.SignalR;

namespace backend_long_running_query.Hubs;

public class QueryHub : Hub
{
    // When a client starts a job, have it call this method to join a group
    // named after its job id.
    public async Task JoinJobGroup(string jobId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, jobId);
    }
}
