using OrderService.Application.Events;
using OrderService.Application.Ports;
using OrderService.Domain.Entities;

namespace OrderService.Application.UseCases;

public class OrderCancelledUseCase : IOrderCancelled
{
    private readonly IOrderRepository _orderRepository;

    public OrderCancelledUseCase(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task HandleAsync(OrderStatus orderStatus)
    {
        Order? order = await _orderRepository.GetByIdAsync(orderStatus.OrderId);
        if (order is not null)
        {
            order.MarkAsCancelled();
            await _orderRepository.UpdateAsync(order);
        }
    }
}