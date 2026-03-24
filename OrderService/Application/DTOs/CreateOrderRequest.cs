using OrderService.Domain.Entities;

namespace OrderService.Application.DTOs;

public class CreateOrderRequest
{
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}