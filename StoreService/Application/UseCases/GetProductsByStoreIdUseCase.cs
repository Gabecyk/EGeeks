using StoreService.Application.DTOs;
using StoreService.Application.Ports;

public class GetProductsByStoreIdUseCase : IGetProductsByStoreIdUseCase
{
    private readonly IStoreRepository _repository;

    public GetProductsByStoreIdUseCase(IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AllProductsDto>> Execute(Guid storeId)
    {
        var existing = await _repository.GetByIdAsync(storeId);
        if (existing == null)
            throw new Exception("Store not found.");
            
        var products = await _repository.GetProductsByStoreIdAsync(storeId);
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