using System.Text.Json;
using OutboxPattern.OrdersManagment.Database;
using OutboxPattern.OrdersManagment.Database.Entities;

namespace OutboxPattern.OrdersManagment.Services;

public class OrdersManagementService : IOrdersManagementService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<OrdersManagementService> _logger;

    public OrdersManagementService(AppDbContext dbContext, ILogger<OrdersManagementService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task CreateOrderAsync(CreateOrderRequest request)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var order = new Order
            {
                UserId = request.UserId,
                ProductsIds = request.ProductIds
            };

            
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            
            var outBoxMessage = new OutBoxMessage
            {
                Payload = JsonSerializer.Serialize(new
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    MessageId = Guid.NewGuid(),
                    Address = "Street Pushkina, Home Colotuskina"
                }),
                IsSent = false
            };

            await _dbContext.OutBoxMessages.AddAsync(outBoxMessage);
            
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            
            _logger.LogInformation("Заказ был создан: {OrderId}", order.Id);
        }
        catch (Exception e)
        {
            _logger.LogCritical("Произошла ошибка при создании заказа: {ExceptionMessage}", e.Message);
            await transaction.RollbackAsync();
        }
    }
}