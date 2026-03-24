using System.Text.Json;
using Azure.Messaging.ServiceBus;
using OrderService.Application.Events;
using OrderService.Application.Ports;

namespace OrderService.Infrastructure.Messaging;

public class AzureServiceBusEventBus : IEventBus, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public AzureServiceBusEventBus(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ServiceBus")
            ?? throw new InvalidOperationException("Connection string do Service Bus não encontrada.");

        var topicName = configuration["ServiceBus:TopicName"]
            ?? throw new InvalidOperationException("TopicName do Service Bus não encontrado.");

        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(topicName);
    }

    public async Task PublishAsync(
        OrderCreatedEvent orderCreatedEvent,
        CancellationToken cancellationToken = default)
    {
        var body = JsonSerializer.Serialize(orderCreatedEvent);        

        var message = new ServiceBusMessage(body)
        {
            Subject = "OrderCreated",
            MessageId = orderCreatedEvent.OrderId.ToString(),
            ContentType = "application/json"
        };

        message.ApplicationProperties["eventType"] = "OrderCreated";
        message.ApplicationProperties["orderId"] = orderCreatedEvent.OrderId.ToString();
        message.ApplicationProperties["customerId"] = orderCreatedEvent.CustomerId.ToString();

        await _sender.SendMessageAsync(message, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}