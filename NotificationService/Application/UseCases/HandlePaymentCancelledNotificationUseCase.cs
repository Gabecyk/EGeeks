using NotificationService.Application.Events;
using NotificationService.Application.Ports;
using NotificationService.Domain.Models;

namespace NotificationService.Application.UseCases;

public class HandlePaymentCancelledNotificationUseCase
{
    private readonly IEmailSender _emailSender;

    public HandlePaymentCancelledNotificationUseCase(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task Execute(PaymentCancelledEvent paymentEvent, CancellationToken cancellationToken = default)
    {
        var email = new EmailMessage
        {
            To = $"cliente-{paymentEvent.CustomerId}@fake.com",
            Subject = "Pagamento cancelado",
            Body = $"O pagamento do pedido {paymentEvent.OrderId} foi cancelado."
        };

        await _emailSender.SendEmailAsync(email, cancellationToken);
    }
}