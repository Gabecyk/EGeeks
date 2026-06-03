using StoreService.Application.DTOs;
using StoreService.Application.Ports;
using StoreService.Domain.Entities;

public class GetProductsByStoreIdUseCase
{
    private readonly IStoreRepository _repository;

    public GetProductsByStoreIdUseCase(IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> Execute(Guid storeId)
    {
        var existing = await _repository.GetByIdAsync(storeId);
        if (existing == null)
            throw new Exception("Store not found.");
            
        var products = await _repository.GetProductsByStoreIdAsync(storeId);
        return products;
    }
}