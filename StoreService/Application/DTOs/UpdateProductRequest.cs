namespace StoreService.Application.DTOs;

public record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string ImageUrl
);
