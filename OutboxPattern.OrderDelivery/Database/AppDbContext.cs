using Microsoft.EntityFrameworkCore;
using OutboxPattern.OrderDelivery.Models;

namespace OutboxPattern.OrderDelivery.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    private AppDbContext()
    {
    }

    public DbSet<ProcessedMessage> ProcessedMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessedMessage>(message =>
        {
            message.HasKey(m => m.MessageId);
        });
    }
}