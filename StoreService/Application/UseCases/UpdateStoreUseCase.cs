using StoreService.Application.DTOs;
using StoreService.Application.Ports;

public class UpdateStoreUseCase
{
    private readonly IStoreRepository _repository;

    public UpdateStoreUseCase(IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task Execute(UpdateStoreRequest request, Guid Id)
    {
        var existing = await _repository.GetByIdAsync(Id);
        if (existing == null)
            throw new Exception("Store not found.");

        existing.UpdateProfile(request.Name, request.Description, request.ProfileImageUrl);

        await _repository.UpdateAsync(existing);
    }
}