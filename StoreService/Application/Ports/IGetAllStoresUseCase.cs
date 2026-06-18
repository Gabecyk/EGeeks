using StoreService.Application.DTOs;

namespace StoreService.Application.Ports;

public interface IGetAllStoresUseCase
{
    Task<List<StoreDto>> Execute();
}