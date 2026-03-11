namespace StoreService.Application.UseCases;

using StoreService.Application.DTOs;
using StoreService.Application.Ports;
using StoreService.Domain.Entities;

public class CreateProductUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly IStoreRepository _storeRepository;

    public CreateProductUseCase(IProductRepository productRepository, IStoreRepository storeRepository)
    {
        _productRepository = productRepository;
        _storeRepository = storeRepository;
    }

    public async Task Execute(CreateProductRequest request, Guid userId)
    {
        var store = await _storeRepository.GetByUserIdAsync(userId);

        if (store is null)
            throw new Exception("Not a store found for this user.");

        var product = new Product(
            store.Id,
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity,
            request.ImageUrl
        );

        await _productRepository.AddAsync(product);
    }
}