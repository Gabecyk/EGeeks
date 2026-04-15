using PaymentService.Application.Events;
using PaymentService.Application.Ports;

namespace PaymentService.Application.UseCases;

public class ConfirmPaymentUseCase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEventBus _eventBus;

    public ConfirmPaymentUseCase(IPaymentRepository paymentRepository, IEventBus eventBus)
    {
        _paymentRepository = paymentRepository;
        _eventBus = eventBus;
    }

    public async Task Execute(Guid orderId, CancellationToken cancellationToken = default)
    {
        var payment =  await _paymentRepository.GetByIdAsync(orderId);
        if (payment is null)
            throw new Exception("Payment not found for the given OrderId.");

        payment.Confirm();
        await _paymentRepository.UpdateAsync(payment);

        var paymentConfirmedEvent = new PaymentConfirmedEvent(
            payment.Id,
            payment.OrderId,
            payment.CustomerId
        );

        await _eventBus.PublishPaymentConfirmedAsync(paymentConfirmedEvent, cancellationToken);
    }
}