using PaymentService.Application.Events;

namespace PaymentService.Application.Events;

public interface IEventBus
{
    Task PublishPaymentCreatedAsync(PaymentCreatedEvent paymentCreatedEvent, CancellationToken cancellationToken = default);
    Task PublishPaymentConfirmedAsync(PaymentConfirmedEvent paymentConfirmedEvent, CancellationToken cancellationToken = default);
    Task PublishPaymentCancelledAsync(PaymentCancelledEvent paymentCancelledEvent, CancellationToken cancellationToken = default);
}