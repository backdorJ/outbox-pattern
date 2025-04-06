using Microsoft.AspNetCore.Mvc;
using OutboxPattern.OrdersManagment.Services;

namespace OutboxPattern.OrdersManagment.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersManagementController : ControllerBase
{
    private readonly IOrdersManagementService _ordersManagementService;

    public OrdersManagementController(IOrdersManagementService ordersManagementService)
        => _ordersManagementService = ordersManagementService;

    [HttpPost("order")]
    public async Task CreateOrder(CreateOrderRequest request)
        => await _ordersManagementService.CreateOrderAsync(request);
}