using OrderService.Application.Events;

namespace OrderService.Application.Ports;

public interface IOrderConfirmed
{
    Task HandleAsync(OrderStatus order);
}