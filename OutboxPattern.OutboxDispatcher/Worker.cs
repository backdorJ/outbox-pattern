using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OutboxPattern.OutboxDispatcher.Database;
using OutboxPattern.OutboxDispatcher.Kafka;

namespace OutboxPattern.OutboxDispatcher;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Producer _producer;
    private readonly IConfiguration _configuration;

    public Worker(
        ILogger<Worker> logger,
        IServiceScopeFactory serviceScopeFactory,
        Producer producer,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _producer = producer;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var messages = await dbContext.OutBoxMessages
                .Where(x => !x.IsSent)
                .Take(50)
                .ToListAsync(cancellationToken: stoppingToken);

            if (!messages.Any())
            {
                await Task.Delay(5000, stoppingToken);
                continue;
            }
            
            foreach (var message in messages)
            {
                try
                {
                    await _producer.ProduceAsync(_configuration["Kafka:OutboxTopic"]!, message.Payload);
                    message.IsSent = true;
                    _logger.LogInformation("Сообщение {MessageId} было отправлено", message.Id);
                }
                catch (Exception e)
                {
                    message.IsSent = false;
                    _logger.LogError("Ошибка отправки сообщения в Kafka {Exception}", e.Message);
                }
            }
            
            await dbContext.SaveChangesAsync(stoppingToken);
            
            await Task.Delay(5000, stoppingToken);
        }
    }
}