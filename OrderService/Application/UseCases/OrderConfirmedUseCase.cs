using OrderService.Application.Events;
using OrderService.Application.Ports;
using OrderService.Domain.Entities;

namespace OrderService.Application.UseCases;

public class OrderConfirmedUseCase : IOrderConfirmed
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventBus _eventBus;

    public OrderConfirmedUseCase(IOrderRepository orderRepository, IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
    }

    public async Task HandleAsync(OrderStatus orderStatus)
    {
        try
        {
            Order? order = await _orderRepository.GetByIdAsync(orderStatus.OrderId);
            if (order is not null)
            {
                order.MarkAsPaid();
                await _orderRepository.UpdateAsync(order);
            }

            List<OrderItem> itens = await _orderRepository.GetOrderItemsByOrderIdAsync(orderStatus.OrderId);

            List<SendProductsConfirmed> itemsToSend = itens.Select(i => new SendProductsConfirmed(
                Id: i.Id,
                ProductId: i.ProductId,
                Quantity: i.Quantity,
                UnitPrice: i.UnitPrice
            )).ToList();

            await _eventBus.PublishOrderConfirmedAsync(itemsToSend);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error confirming order: {ex.Message}", ex);
        }
    }
}