using OrderService.Domain.Entities;

namespace OrderService.Application.Ports;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<List<Order>> GetByCustomerIdAsync(Guid customerId);
    Task<Order?> GetByIdAsync(Guid id);
}