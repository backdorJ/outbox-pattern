using Microsoft.EntityFrameworkCore;
using OutboxPattern.OutboxDispatcher;
using OutboxPattern.OutboxDispatcher.Database;
using OutboxPattern.OutboxDispatcher.Kafka;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration["PostgreSQL"]));
builder.Services.AddSingleton<Producer>(sp => new Producer(builder.Configuration));

var host = builder.Build();
host.Run();