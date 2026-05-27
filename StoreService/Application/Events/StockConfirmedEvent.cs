namespace StoreService.Application.Events;

public record StockConfirmedEvent(
    List<StockItem> StockItems
);

public record StockItem(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice
);