using OrderService.Application.DTOs;

namespace OrderService.Application.Ports;

public interface IProcessServiceBusMessageUseCase
{
    Task ExecuteAsync(ServiceBusOrderEventCommand command, CancellationToken cancellationToken = default);
}
