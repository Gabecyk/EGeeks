using OrderService.Domain.Entities;

namespace OrderService.Application.DTOs;

public class CreateOrderRequest
{
    public Guid CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
}