namespace OutboxPattern.OrdersManagment.Database.Entities;

public class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public List<Guid> ProductsIds { get; set; }
}