using OrderService.Application.Events;

namespace OrderService.Application.Ports;

public interface IEventBus
{
    Task PublishAsync(OrderCreatedEvent orderCreatedEvent, CancellationToken cancellationToken = default);
}