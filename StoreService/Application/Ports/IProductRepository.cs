namespace StoreService.Application.Ports;

using StoreService.Domain.Entities;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    Task<List<Product>> GetAllAsync();
    Task DeleteAsync(Product product);
}