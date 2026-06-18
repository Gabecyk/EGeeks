using StoreService.Application.DTOs;

namespace StoreService.Application.Ports;

public interface IGetProductByIdUseCase
{
    Task<ProductDto?> Execute(Guid id);
}