using OrderService.Application.DTOs;
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

    public async Task<Guid> Execute(CreateOrderRequest request, Guid customerId)
    {
        var items = request.Items.Select(i => 
            new  OrderItem(i.ProductId, i.Quantity, i.UnitPrice)
        ).ToList();

        var order = new Order(customerId, items);

        await _orderRepository.AddAsync(order);
        
        await _eventBus.PublishAsync("OrderCreated", new
        {
            OrderId = order.Id,
            CustomerId = customerId,
            order.TotalAmount
        });

        return order.Id;
    }
}