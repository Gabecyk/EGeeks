using OrderService.Application.DTOs;

namespace OrderService.Application.Ports;

public interface IProcessServiceBusMessage
{
    Task ExecuteAsync(ServiceBusOrderEventCommand command, CancellationToken cancellationToken = default);
}
