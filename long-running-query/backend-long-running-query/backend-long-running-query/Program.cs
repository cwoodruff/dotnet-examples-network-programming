using backend_long_running_query.Data;
using backend_long_running_query.Hubs;
using backend_long_running_query.Services;
using LongRunningQueryAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure EF Core (adjust the connection string as needed).
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register API controllers.
builder.Services.AddControllers();

// Register SignalR.
builder.Services.AddSignalR();

// Register the background job queue as a singleton.
builder.Services.AddSingleton<IBackgroundJobQueue, BackgroundJobQueue>();

// Register the background processing service.
builder.Services.AddHostedService<QueryProcessingService>();

// Configure CORS (optional, adjust as needed for your clients).
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();

// Enable developer-friendly error pages in development.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapHub<QueryHub>("/queryHub");

app.Run();