using PaymentService.Application.Events;
using PaymentService.Application.Ports;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.UseCases;

public class CreatePaymentFromOrderUseCase
{
    public readonly IPaymentRepository _paymentRepository;
    private readonly IEventBus _eventBus;
    
    public CreatePaymentFromOrderUseCase(IPaymentRepository paymentRepository, IEventBus eventBus)
    {
        _paymentRepository = paymentRepository;
        _eventBus = eventBus;
    }

    public async Task Execute(OrderCreatedEvent orderEvent, CancellationToken cancellationToken = default)
    {
        var existingPayment = await _paymentRepository.GetByOrderIdAsync(orderEvent.OrderId);
        if (existingPayment is not null)
            return;

        var fakeBoletoCode = $"BOL-{Guid.NewGuid():N}".ToUpperInvariant();

        var payment = new Payment(
            orderEvent.OrderId,
            orderEvent.CustomerId,
            orderEvent.TotalAmount,
            fakeBoletoCode
        );
        
        await _paymentRepository.AddAsync(payment);

        var paymentCreatedEvent = new PaymentCreatedEvent(
            payment.Id,
            payment.OrderId,
            payment.CustomerId,
            payment.Amount,
            payment.FakeBoletoCode
        );

        await _eventBus.PublishPaymentCreatedAsync(paymentCreatedEvent, cancellationToken);
    }
}