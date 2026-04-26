using NotificationService.Domain.Models;

namespace NotificationService.Application.Ports;

public interface ICustomerClient
{
    Task<CustomerData?> GetCustomerEmailAsync(Guid customerId, CancellationToken cancellationToken = default);
}