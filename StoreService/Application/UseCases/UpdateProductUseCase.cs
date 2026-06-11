using StoreService.Application.DTOs;
using StoreService.Application.Ports;

namespace StoreService.Application.UseCases;

public class UpdateProductUseCase
{
    private readonly IProductRepository _repository;
    private readonly IStoreRepository _storeRepository;

    public UpdateProductUseCase(IProductRepository productRepository, IStoreRepository storeRepository)
    {
        _repository = productRepository;
        _storeRepository = storeRepository;
    }

    public async Task Execute(Guid productId, UpdateProductRequest request, Guid userId)
    {
        var store = await _storeRepository.GetByUserIdAsync(userId);
        if (store is null)
            throw new Exception("Not a store found for this user.");

        var product = await _repository.GetByIdAsync(productId);
        if (product is null || product.StoreId != store.Id)
            throw new Exception("Product not found for this store.");

        product.Update(request.Name, request.Description, request.Price, request.StockQuantity, request.ImageUrl);

        await _repository.UpdateAsync(product);
    }
}
