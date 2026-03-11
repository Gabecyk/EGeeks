namespace OrderService.Application.Ports;

public interface IEventBus
{
    Task PublishAsync(string eventName, object payload);
}