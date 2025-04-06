using Microsoft.EntityFrameworkCore;
using OutboxPattern.OutboxDispatcher.Database.Entities;

namespace OutboxPattern.OutboxDispatcher.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<OutBoxMessage> OutBoxMessages { get; set; }
}