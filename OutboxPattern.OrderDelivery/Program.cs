using Microsoft.EntityFrameworkCore;
using OutboxPattern.OrderDelivery;
using OutboxPattern.OrderDelivery.Database;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration["PostgreSql"]));
builder.Services.AddMemoryCache();

var host = builder.Build();

host.Run();