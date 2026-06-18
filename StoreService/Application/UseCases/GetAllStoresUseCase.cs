using StoreService.Application.Ports;
using StoreService.Application.DTOs;

namespace StoreService.Application.UseCases;

public class GetAllStoresUseCase : IGetAllStoresUseCase
{
    private readonly IStoreRepository _repository;

    public GetAllStoresUseCase(IStoreRepository storeRepository)
    {
        _repository = storeRepository;
    }

    public async Task<List<StoreDto>> Execute()
    {
        var stores = await _repository.GetAllAsync();
        return stores.Select(s => new StoreDto(
            s.Id,
            s.UserId,
            s.Name,
            s.Description,
            s.ProfileImageUrl,
            s.CreatedAt
        )).ToList();
    }
}