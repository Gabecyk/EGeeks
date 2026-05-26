namespace OrderService.Application.Events;
public record SendProductsConfirmed(
    Guid Id,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice
);