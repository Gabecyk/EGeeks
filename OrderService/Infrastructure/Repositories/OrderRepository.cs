using Microsoft.EntityFrameworkCore;
using OrderService.Application.Ports;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Data;

namespace OrderService.Infrastructure.Persistence;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;

    public OrderRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Order order)
    {
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Order?> GetByIdAsync(Guid id)
        => await _dbContext.Orders.FindAsync(id);

    public async Task<List<Order>> GetByCustomerIdAsync(Guid customerId)
        => await _dbContext.Orders.Where(o => o.CustomerId == customerId).ToListAsync();

    public async Task UpdateAsync(Order order)
    {
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync();
    }
}