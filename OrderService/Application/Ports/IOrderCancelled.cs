using OrderService.Application.Events;

namespace OrderService.Application.Ports;

public interface IOrderCancelled
{
    Task HandleAsync(OrderStatus order);
}