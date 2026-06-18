using StoreService.Application.DTOs;

namespace StoreService.Application.Ports;

public interface IGetProductsByStoreIdUseCase
{
    Task<List<AllProductsDto>> Execute(Guid storeId);
}