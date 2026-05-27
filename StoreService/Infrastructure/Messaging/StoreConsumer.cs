using System.Text.Json;
using Azure.Messaging.ServiceBus;
using StoreService.Application.Ports;
using StoreService.Application.DTOs;
using StoreService.Application.Events;

namespace StoreService.Infrastructure.Messaging;

public class StoreConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;

    public StoreConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString("ServiceBus")
            ?? throw new InvalidOperationException("Azure Service Bus connection string is not configured.");

        var topicName = _configuration["ServiceBus:TopicName"]
            ?? throw new InvalidOperationException("Azure Service Bus topic name is not configured.");

        var subscriptionName = _configuration["ServiceBus:SubscriptionName"]
            ?? throw new InvalidOperationException("Azure Service Bus subscription name is not configured.");

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

            if (string.IsNullOrEmpty(eventType))
            {
                await args.DeadLetterMessageAsync(args.Message, "Missing eventType property");
                return;
            }

            var body = args.Message.Body.ToString();
            var orderStatus = DeserializeMessage(body);
            if (orderStatus == null)
            {
                await args.DeadLetterMessageAsync(args.Message, "Invalid message body");
                return;
            }

            var command = new StockConfirmedDto(eventType, orderStatus);

            await using var scope = _scopeFactory.CreateAsyncScope();
            var useCase = scope.ServiceProvider.GetRequiredService<IStockConfirmed>();
            await useCase.HandleStockConfirmedAsync(command);

            await args.CompleteMessageAsync(args.Message);
        }
        catch (JsonException ex)
        {
            await args.DeadLetterMessageAsync(args.Message, $"Invalid message format: {ex.Message}");
        }
        catch (Exception ex)
        {
            await args.DeadLetterMessageAsync(args.Message, $"Error processing message: {ex.Message}");
        }
    }

    private StockConfirmedEvent? DeserializeMessage(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        using var document = JsonDocument.Parse(body);

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            var items = document.RootElement.Deserialize<List<StockItem>>(_jsonOptions);
            return items is null ? null : new StockConfirmedEvent(items);
        }

        return document.RootElement.Deserialize<StockConfirmedEvent>(_jsonOptions);
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