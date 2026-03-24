using OrderService.Application.DTOs;
using OrderService.Application.Events;
using OrderService.Application.Ports;
using OrderService.Domain.Entities;

namespace OrderService.Application.UseCases;

public class CreateOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventBus _eventBus;

    public CreateOrderUseCase(IOrderRepository orderRepository, IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
    }

    public async Task<Guid> Execute(CreateOrderRequest request, Guid customerId, CancellationToken cancellationToken = default)
    {
        var items = request.Items.Select(i => 
            new  OrderItem(i.ProductId, i.Quantity, i.UnitPrice)
        ).ToList();

        var order = new Order(customerId, items);

        await _orderRepository.AddAsync(order);
        
        var orderCreatedEvent = new OrderCreatedEvent(
            order.Id,
            customerId,
            order.TotalAmount,
            items.Select(i => new OrderCreatedItem(
                i.ProductId,
                i.Quantity,
                i.UnitPrice
            )).ToList()
        );

        await _eventBus.PublishAsync(orderCreatedEvent, cancellationToken);

        return order.Id;
    }
}