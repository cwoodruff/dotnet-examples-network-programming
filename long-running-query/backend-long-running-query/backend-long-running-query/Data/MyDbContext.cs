using Microsoft.EntityFrameworkCore;

namespace backend_long_running_query.Data;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    // DbSet for your entities.
    public DbSet<MyEntity> MyEntities { get; set; }
}

// Sample entity for demonstration.
public class MyEntity
{
    public int Id { get; set; }
    public string SomeProperty { get; set; }
}