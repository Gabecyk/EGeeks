using StoreService.Application.Events;
using StoreService.Application.Ports;
using StoreService.Application.DTOs;
using StoreService.Domain.Entities;

namespace StoreService.Application.UseCases;

public class StockConfirmedUseCase : IStockConfirmed
{
    private readonly IProductRepository _productRepository;

    public StockConfirmedUseCase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task HandleStockConfirmedAsync(StockConfirmedDto stockConfirmedDto)
    {
        if (stockConfirmedDto.EventType != "OrderConfirmed")
        {
            throw new ArgumentException("Invalid event type");
        }

        try
        {
            foreach (var item in stockConfirmedDto.EventData.StockItems)
            {
                Product? product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product with ID {item.ProductId} not found");
                }

                if (product.StockQuantity < item.Quantity)
                {
                    throw new Exception($"Insufficient stock for product {product.Name}");
                }

                product.ReduceStock(item.Quantity);
                await _productRepository.UpdateAsync(product);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error handling stock confirmed event: {ex.Message}");
        }
    }
}