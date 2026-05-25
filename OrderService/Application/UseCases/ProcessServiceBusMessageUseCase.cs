using OrderService.Application.DTOs;
using OrderService.Application.Ports;

namespace OrderService.Application.UseCases;

public class ProcessServiceBusMessageUseCase : IProcessServiceBusMessageUseCase
{
    private readonly IOrderConfirmed _orderConfirmed;
    private readonly IOrderCancelled _orderCancelled;

    public ProcessServiceBusMessageUseCase(IOrderConfirmed orderConfirmed, IOrderCancelled orderCancelled)
    {
        _orderConfirmed = orderConfirmed;
        _orderCancelled = orderCancelled;
    }

    public async Task ExecuteAsync(ServiceBusOrderEventCommand command, CancellationToken cancellationToken = default)
    {
        if (command.OrderStatus is null)
        {
            throw new InvalidOperationException("Payload do evento não pode ser nulo.");
        }

        switch (command.EventType)
        {
            case "PaymentConfirmed":
                await _orderConfirmed.HandleAsync(command.OrderStatus);
                break;
            case "PaymentCancelled":
                await _orderCancelled.HandleAsync(command.OrderStatus);
                break;
            default:
                throw new InvalidOperationException($"Evento desconhecido: {command.EventType}");
        }
    }
}
