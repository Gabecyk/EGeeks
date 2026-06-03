using StoreService.Application.DTOs;
using StoreService.Application.Ports;
using StoreService.Domain.Entities;

public class GetStoreByStoreIdForCustomerUseCase
{
    private readonly IStoreRepository _repository;

    public GetStoreByStoreIdForCustomerUseCase(IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task<Store?> Execute(Guid storeId)
    {
        var existing = await _repository.GetByStoreIdAsync(storeId);
        if (existing == null)
            throw new Exception("Store not found.");

        return existing;
    }
}