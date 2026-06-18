using StoreService.Application.Ports;
using StoreService.Application.DTOs;

namespace StoreService.Application.UseCases;

public class GetAllProductsUseCase : IAllProductsUseCase
{
    private readonly IProductRepository _repository;

    public GetAllProductsUseCase(IProductRepository productRepository)
    {
        _repository = productRepository;
    }

    public async Task<List<AllProductsDto>> Execute()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(p => new AllProductsDto(
            p.Id,
            p.StoreId,
            p.Name,
            p.Price,
            p.StockQuantity,
            p.ImageUrl
        )).ToList();
    }
}