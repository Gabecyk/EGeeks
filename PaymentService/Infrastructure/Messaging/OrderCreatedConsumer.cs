using System.Text.Json;
using Azure.Messaging.ServiceBus;
using PaymentService.Application.Events;
using PaymentService.Application.UseCases;

namespace PaymentService.Infrastructure.Messaging;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private ServiceBusProcessor? _processor;

    public OrderCreatedConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("AzureServiceBus")
            ?? throw new InvalidOperationException("Azure Service Bus connection string is not configured.");

        var topicName = _configuration["ServiceBus:TopicName"]
            ?? throw new InvalidOperationException("Azure Service Bus topic name is not configured.");

        var subscriptionName = _configuration["ServiceBus:PaymentSubscriptionName"]
            ?? throw new InvalidOperationException("Azure Service Bus subscription name is not configured.");

        var client = new ServiceBusClient(connectionString);

        _processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;

        await _processor.StartProcessingAsync(cancellationToken);

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var eventType = args.Message.Subject;

        if (eventType != "OrderCreated")
        {
            await args.CompleteMessageAsync(args.Message);
            return;
        }

        var body = args.Message.Body.ToString();
        var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(body);

        if (orderCreatedEvent is null)
        {
            await args.DeadLetterMessageAsync(args.Message, "InvalidPayload, error deserializing OrderCreatedEvent");
            return;
        }

        await using var scope = _scopeFactory.CreateAsyncScope();
        var useCase = scope.ServiceProvider.GetRequiredService<CreatePaymentFromOrderUseCase>();

        await useCase.Execute(orderCreatedEvent);

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

        await base.StopAsync(cancellationToken);
    }
}