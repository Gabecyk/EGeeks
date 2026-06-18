namespace StoreService.Application.DTOs;

public record ProductDto
(
    Guid Id,
    Guid StoreId,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string ImageUrl,
    DateTime CreatedAt,
    DateTime UpdateAt
);