using System.Text.Json;
using Azure.Messaging.ServiceBus;
using OrderService.Application.DTOs;
using OrderService.Application.Events;
using OrderService.Application.Ports;

namespace OrderService.Infrastructure.Messaging;

public class OrderConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;

    public OrderConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("ServiceBus")
            ?? throw new InvalidOperationException("Connection string do Service Bus não encontrada.");

        var topicName = _configuration["ServiceBus:TopicName"]
            ?? throw new InvalidOperationException("TopicName do Service Bus não encontrado.");

        var subscriptionName = _configuration["ServiceBus:SubscriptionName"]
            ?? throw new InvalidOperationException("SubscriptionName do Service Bus não encontrado.");

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
        try
        {
            var eventType = args.Message.ApplicationProperties.TryGetValue("eventType", out var eventTypeValue)
                ? eventTypeValue?.ToString()
                : null;

            if (string.IsNullOrWhiteSpace(eventType))
            {
                await args.DeadLetterMessageAsync(args.Message, "MissingEventType", "ApplicationProperties.eventType não informado.");
                return;
            }

            var orderStatus = JsonSerializer.Deserialize<OrderStatus>(args.Message.Body.ToString());
            var command = new ServiceBusOrderEventCommand(eventType, orderStatus);

            await using var scope = _scopeFactory.CreateAsyncScope();
            var useCase = scope.ServiceProvider.GetRequiredService<IProcessServiceBusMessage>();
            await useCase.ExecuteAsync(command);

            await args.CompleteMessageAsync(args.Message);
        }
        catch (JsonException)
        {
            await args.DeadLetterMessageAsync(args.Message, "InvalidPayload", "Payload inválido para o evento recebido.");
        }
        catch (InvalidOperationException ex)
        {
            await args.DeadLetterMessageAsync(args.Message, "InvalidEvent", ex.Message);
        }
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