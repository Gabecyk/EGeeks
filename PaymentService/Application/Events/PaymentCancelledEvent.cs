namespace PaymentService.Application.Events;

public record PaymentCancelledEvent(
    Guid PaymentId,
    Guid OrderId,
    Guid CustomerId
);