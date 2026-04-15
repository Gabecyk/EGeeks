using PaymentService.Application.Events;
using PaymentService.Application.Ports;

namespace PaymentService.Application.UseCases;

public class CancelPaymentUseCase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEventBus _eventBus;

    public CancelPaymentUseCase(IPaymentRepository paymentRepository, IEventBus eventBus)
    {
        _paymentRepository = paymentRepository;
        _eventBus = eventBus;
    }

    public async Task Execute(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);
        if (payment is null)
            throw new Exception("Payment not found for the given PaymentId.");

        payment.Cancel();
        await _paymentRepository.UpdateAsync(payment);

        var paymentCancelledEvent = new PaymentCancelledEvent(
            payment.Id,
            payment.OrderId,
            payment.CustomerId
        );

        await _eventBus.PublishPaymentCancelledAsync(paymentCancelledEvent, cancellationToken);
    }
}