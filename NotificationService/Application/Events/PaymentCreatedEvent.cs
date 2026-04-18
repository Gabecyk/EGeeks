namespace NotificationService.Application.Events;

public record PaymentCreatedEvent(
    Guid PaymentId,
    Guid OrderId,
    Guid CustomerId,
    decimal Amount,
    string FakeBoletoCode
);