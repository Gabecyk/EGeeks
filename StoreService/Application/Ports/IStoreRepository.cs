namespace StoreService.Application.Ports;

using StoreService.Domain.Entities;

public interface IStoreRepository
{
    Task AddAsync(Store store);
    Task<Store?> GetByUserIdAsync(Guid userId);
}