namespace OutboxPattern.OrdersManagment.Services;

public interface IOrdersManagementService
{
    public Task CreateOrderAsync(CreateOrderRequest request);
}

public record CreateOrderRequest(List<Guid> ProductIds, Guid UserId);