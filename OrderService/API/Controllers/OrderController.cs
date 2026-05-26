using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.UseCases;

namespace OrderService.API.Controllers;

[ApiController]
[Authorize(Roles = "Customer")]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    [Authorize(Roles = "Customer")]
    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        CreateOrderRequest request,
        [FromServices] CreateOrderUseCase useCase,
        CancellationToken cancellationToken)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier) is string id 
            ? Guid.Parse(id) 
            : throw new InvalidOperationException("Customer ID not found in token.");

        var orderId = await useCase.Execute(request, customerId, cancellationToken);

        return Ok(new { orderId });
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("my-orders")]
    public async Task<IActionResult> MyOrders(
        [FromServices] GetCustomerOrdersUseCase useCase)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier) is string id 
            ? Guid.Parse(id) 
            : throw new InvalidOperationException("Customer ID not found in token.");

        var orders = await useCase.Execute(customerId);

        return Ok(orders);
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromServices] GetOrderByIdUseCase useCase
    )
    {
        var order = await useCase.Execute(id);
        return Ok(order);
    }
}