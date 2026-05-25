namespace OrderService.Application.Events;
public record OrderStatus(
    Guid PaymentId,
    Guid OrderId,
    Guid CustomerId
);