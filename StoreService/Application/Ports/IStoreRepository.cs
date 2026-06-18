namespace StoreService.Application.Ports;

using StoreService.Domain.Entities;

public interface IStoreRepository
{
    Task AddAsync(Store store);
    Task<Store?> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(Store store);
    Task<Store?> GetByIdAsync(Guid Id);
    Task<IEnumerable<Product>> GetProductsByStoreIdAsync(Guid storeId);
    Task<Store?> GetByStoreIdAsync(Guid storeId);
    Task<List<Store>> GetAllAsync();
}