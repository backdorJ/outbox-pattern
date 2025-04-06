using System.Text.Json;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OutboxPattern.OrderDelivery.Database;
using OutboxPattern.OrderDelivery.Models;

namespace OutboxPattern.OrderDelivery;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public Worker(
        ILogger<Worker> logger,
        IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig()
        {
            GroupId = "OrderGroup-2",
            BootstrapServers = _configuration["Kafka:BootstrapServers"]!,
            EnableAutoCommit = false,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };
        
        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_configuration["Kafka:OutboxTopic"]);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                var result = consumer.Consume(TimeSpan.FromSeconds(5));

                if (result == null)
                    continue;
                
                var message = JsonSerializer.Deserialize<OrderMessage>(result.Message.Value);
                
                if (message == null)
                    continue;
                
                if (await dbContext.ProcessedMessages.AnyAsync(m => m.MessageId == message.MessageId, stoppingToken))
                    continue;
                
                
                // to do logic
                
                await dbContext.ProcessedMessages.AddAsync(new ProcessedMessage
                {
                    MessageId = message.MessageId,
                }, stoppingToken);
                await dbContext.SaveChangesAsync(stoppingToken);
                
                if (Rool(0.75))
                    throw new Exception("Something wrong");
                
                consumer.Commit(result);
                _logger.LogInformation("Доставка создана!");
            }
            catch (Exception e)
            {
                _logger.LogError("Произошла непредвиденная ошибка");
            }
            
            
            await Task.Delay(1000, stoppingToken);
        }
    }
    
    private bool Rool(double chance)
        => Random.Shared.NextDouble() < chance;
}