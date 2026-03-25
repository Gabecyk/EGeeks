namespace PaymentService.Application.Events;

public record PaymentConfirmedEvent(
    Guid PaymentId,
    Guid OrderId,
    Guid CustomerId
);