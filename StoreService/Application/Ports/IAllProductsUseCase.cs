using StoreService.Application.DTOs;

namespace StoreService.Application.Ports;

public interface IAllProductsUseCase
{
    Task<List<AllProductsDto>> Execute();
}