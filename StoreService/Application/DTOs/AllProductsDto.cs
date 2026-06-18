namespace StoreService.Application.DTOs;

public record AllProductsDto
(
    Guid Id,
    Guid StoreId,
    string Name,
    decimal Price,
    int StockQuantity,
    string ImageUrl
);