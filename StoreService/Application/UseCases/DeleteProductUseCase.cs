using StoreService.Application.Ports;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases;

public class DeleteProductUseCase
{
    private readonly IProductRepository _repository;
    private readonly IStoreRepository _storeRepository;

    public DeleteProductUseCase(IProductRepository productRepository, IStoreRepository storeRepository)
    {
        _repository = productRepository;
        _storeRepository = storeRepository;
    }

    public async Task Execute(Guid productId, Guid userId)
    {
        var store = await _storeRepository.GetByUserIdAsync(userId);
        if (store is null)
            throw new Exception("Not a store found for this user.");

        var product = await _repository.GetByIdAsync(productId);
        if (product is null || product.StoreId != store.Id)
            throw new Exception("Product not found for this store.");

        await _repository.DeleteAsync(product);
    }
}