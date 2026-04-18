namespace NotificationService.Application.Events;

public record PaymentConfirmedEvent(
    Guid PaymentId,
    Guid OrderId,
    Guid CustomerId
);