using NotificationService.Application.Events;
using NotificationService.Application.Ports;
using NotificationService.Domain.Models;

namespace NotificationService.Application.UseCases;

public class HandlePaymentConfirmedNotificationUseCase
{
    private readonly IEmailSender _emailSender;

    public HandlePaymentConfirmedNotificationUseCase(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task Execute(PaymentConfirmedEvent paymentEvent, CancellationToken cancellationToken = default)
    {
        var customerEmail = new EmailMessage
        {
            To = $"cliente-{paymentEvent.CustomerId}@fake.com",
            Subject = "Pagamento confirmado",
            Body = $"Seu pagamento do pedido {paymentEvent.OrderId} foi confirmado com sucesso."
        };

        var storeEmail = new EmailMessage
        {
            To = $"loja-pedido-{paymentEvent.OrderId}@fake.com",
            Subject = "Pedido concluído",
            Body = $"O pedido {paymentEvent.OrderId} foi pago e está concluído."
        };

        await _emailSender.SendEmailAsync(customerEmail, cancellationToken);
        await _emailSender.SendEmailAsync(storeEmail, cancellationToken);
    }
}