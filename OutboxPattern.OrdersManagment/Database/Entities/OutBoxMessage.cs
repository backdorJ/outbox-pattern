namespace OutboxPattern.OrdersManagment.Database.Entities;

public class OutBoxMessage
{
    public Guid Id { get; set; }
    public string Payload { get; set; }
    public bool IsSent { get; set; }
}