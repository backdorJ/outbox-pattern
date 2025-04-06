using Microsoft.EntityFrameworkCore;
using OutboxPattern.OrdersManagment.Database.Entities;

namespace OutboxPattern.OrdersManagment.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    private AppDbContext()
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OutBoxMessage> OutBoxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(order =>
        {
            order.HasKey(x => x.Id);
            order.Property(x => x.ProductsIds)
                .HasColumnType("text[]");
            order.Property(x => x.UserId);
        });

        modelBuilder.Entity<OutBoxMessage>(message =>
        {
            message.HasKey(x => x.Id);
            message.Property(x => x.Payload);
            message.Property(x => x.IsSent);
        });
    }
}