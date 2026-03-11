using StoreService.Application.DTOs;
using StoreService.Application.Ports;
using StoreService.Domain.Entities;

public class CreateStoreUseCase
{
    private readonly IStoreRepository _repository;

    public CreateStoreUseCase(IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task Execute(CreateStoreRequest request, Guid userId)
    {
        var existing = await _repository.GetByUserIdAsync(userId);
        if (existing != null)
            throw new Exception("This user already has a store.");

        var store = new Store(userId, request.Name,
            request.Description, request.ProfileImageUrl);

        await _repository.AddAsync(store);
    }
}