using StoreService.Application.Ports;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases;

public class GetAllProductsUseCase
{
    private readonly IProductRepository _repository;

    public GetAllProductsUseCase(IProductRepository productRepository)
    {
        _repository = productRepository;
    }

    public async Task<List<Product>> Execute()
    {
        return await _repository.GetAllAsync();
    }
}