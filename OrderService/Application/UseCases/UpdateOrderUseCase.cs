using OrderService.Application.Ports;
using OrderService.Domain.Entities;

namespace OrderService.Application.UseCases;

public class UpdateOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventBus _eventBus;

    public UpdateOrderUseCase(IOrderRepository orderRepository, IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
    }

    public async Task Execute(Order order)
    {
        await _orderRepository.UpdateAsync(order);
        await _eventBus.PublishAsync("OrderUpdated", new
        {
            OrderId = order.Id,
            order.CustomerId,
            order.TotalAmount
        });
    }
}