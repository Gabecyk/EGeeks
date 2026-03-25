namespace PaymentService.Application.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    List<OrderCreatedItem> Items
);

public record OrderCreatedItem(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice
);