using StoreService.Application.DTOs;
using StoreService.Application.Ports;

namespace StoreService.Application.UseCases;

public class GetProductByIdUseCase : IGetProductByIdUseCase
{
    private readonly IProductRepository _repository;

    public GetProductByIdUseCase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto?> Execute(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            return null;

        return new ProductDto(
            product.Id,
            product.StoreId,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.ImageUrl,
            product.CreatedAt,
            product.UpdateAt
        );
    }
}