using OrderService.Application.Ports;

namespace OrderService.Infrastructure.Messaging;

public class FakeEventBus : IEventBus
{
    public Task PublishAsync(string eventName, object payload)
    {
        // Simulate publishing an event for now.
        Console.WriteLine($"Event Published: {eventName} with payload: {payload}");
        return Task.CompletedTask;
    }
}