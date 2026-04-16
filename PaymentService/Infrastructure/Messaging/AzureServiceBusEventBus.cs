using System.Text.Json;
using Azure.Messaging.ServiceBus;
using PaymentService.Application.Ports;
using PaymentService.Application.Events;

namespace PaymentService.Infrastructure.Messaging;

public class AzureServiceBusEventBus : IEventBus, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public AzureServiceBusEventBus(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureServiceBus")
            ?? throw new InvalidOperationException("Azure Service Bus connection string is not configured.");
        
        var topicName = configuration["ServiceBus:TopicName"] 
            ?? throw new InvalidOperationException("Azure Service Bus topic name is not configured.");

        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(topicName);
    }

    public Task PublishPaymentCreatedAsync(PaymentCreatedEvent paymentCreatedEvent, CancellationToken cancellationToken = default)
        => PublishAsync("PaymentCreated", paymentCreatedEvent, paymentCreatedEvent.OrderId, paymentCreatedEvent.CustomerId, cancellationToken);

    public Task PublishPaymentConfirmedAsync(PaymentConfirmedEvent paymentConfirmedEvent, CancellationToken cancellationToken = default)
        => PublishAsync("PaymentConfirmed", paymentConfirmedEvent, paymentConfirmedEvent.OrderId, paymentConfirmedEvent.CustomerId, cancellationToken);

    public Task PublishPaymentCancelledAsync(PaymentCancelledEvent paymentCancelledEvent, CancellationToken cancellationToken = default)
        => PublishAsync("PaymentCancelled", paymentCancelledEvent, paymentCancelledEvent.OrderId, paymentCancelledEvent.CustomerId, cancellationToken);

    private async Task PublishAsync(string eventType, object payload, Guid orderId, Guid customerId, CancellationToken cancellationToken)
    {
        var body = JsonSerializer.Serialize(payload);

        var message = new ServiceBusMessage(body)
        {
            Subject = eventType,
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString(),
        };

        message.ApplicationProperties["eventType"] = eventType;
        message.ApplicationProperties["orderId"] = orderId.ToString();
        message.ApplicationProperties["customerId"] = customerId.ToString();

        await _sender.SendMessageAsync(message, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}