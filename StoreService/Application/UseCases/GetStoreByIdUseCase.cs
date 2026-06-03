using StoreService.Application.DTOs;
using StoreService.Application.Ports;
using StoreService.Domain.Entities;

public class GetStoreByIdUseCase
{
    private readonly IStoreRepository _repository;

    public GetStoreByIdUseCase(IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task<Store?> Execute(Guid userId)
    {
        var existing = await _repository.GetByUserIdAsync(userId);
        if (existing == null)
            throw new Exception("Store not found.");

        return existing;
    }
}