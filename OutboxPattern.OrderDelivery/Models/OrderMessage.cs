namespace OutboxPattern.OrderDelivery.Models;

public class OrderMessage
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public Guid MessageId { get; set; }
    public string Address { get; set; }
}