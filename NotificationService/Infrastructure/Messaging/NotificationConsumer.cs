using System.Text.Json;
using Azure.Messaging.ServiceBus;
using NotificationService.Application.Events;
using NotificationService.Application.UseCases;

namespace NotificationService.Infrastructure.Messaging;

public class NotificationConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;

    public NotificationConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("AzureServiceBus")
            ?? throw new InvalidOperationException("Service Bus connection string not found.");

        var topicName = _configuration["ServiceBus:TopicName"]
            ?? throw new InvalidOperationException("Service Bus topic name not found.");

        var subscriptionName = _configuration["ServiceBus:SubscriptionName"]
            ?? throw new InvalidOperationException("Service Bus subscription name not found.");

        _client = new ServiceBusClient(connectionString);
        _processor = _client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;

        await _processor.StartProcessingAsync(cancellationToken);

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var eventType = args.Message.Subject;

        await using var scope = _scopeFactory.CreateAsyncScope();

        switch (eventType)
        {
            case "PaymentCreated":
                {
                    var body = args.Message.Body.ToString();
                    var paymentCreatedEvent = JsonSerializer.Deserialize<PaymentCreatedEvent>(body);

                    if (paymentCreatedEvent is null)
                    {
                        await args.DeadLetterMessageAsync(args.Message, "DeserializationFailed", "Failed to deserialize PaymentCreatedEvent");
                        return;
                    }

                    var useCase = scope.ServiceProvider.GetRequiredService<HandlePaymentCreatedNotificationUseCase>();
                    await useCase.Execute(paymentCreatedEvent);
                    break;
                }
            case "PaymentConfirmed":
                {
                    var body = args.Message.Body.ToString();
                    var paymentConfirmedEvent = JsonSerializer.Deserialize<PaymentConfirmedEvent>(body);

                    if (paymentConfirmedEvent is null)
                    {
                        await args.DeadLetterMessageAsync(args.Message, "DeserializationFailed", "Failed to deserialize PaymentConfirmedEvent");
                        return;
                    }

                    var useCase = scope.ServiceProvider.GetRequiredService<HandlePaymentConfirmedNotificationUseCase>();
                    await useCase.Execute(paymentConfirmedEvent);
                    break;
                }
            case "PaymentCancelled":
                {
                    var body = args.Message.Body.ToString();
                    var paymentCancelledEvent = JsonSerializer.Deserialize<PaymentCancelledEvent>(body);

                    if (paymentCancelledEvent is null)
                    {
                        await args.DeadLetterMessageAsync(args.Message, "DeserializationFailed", "Failed to deserialize PaymentCancelledEvent");
                        return;
                    }

                    var useCase = scope.ServiceProvider.GetRequiredService<HandlePaymentCancelledNotificationUseCase>();
                    await useCase.Execute(paymentCancelledEvent);
                    break;
                }

            default:
                await args.DeadLetterMessageAsync(args.Message, "UnknownEventType", "Unknown event type");
                break;
        }

        await args.CompleteMessageAsync(args.Message);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Error processing message: {args.Exception}");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor is not null)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        if (_client is not null)
        {
            await _client.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}