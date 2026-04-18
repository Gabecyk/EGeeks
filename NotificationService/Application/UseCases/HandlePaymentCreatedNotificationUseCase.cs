using NotificationService.Application.Events;
using NotificationService.Application.Ports;
using NotificationService.Domain.Models;

namespace NotificationService.Application.UseCases;

public class HandlePaymentCreatedNotificationUseCase
{
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public HandlePaymentCreatedNotificationUseCase(
        IEmailSender emailSender,
        IConfiguration configuration)
    {
        _emailSender = emailSender;
        _configuration = configuration;
    }

    public async Task Execute(PaymentCreatedEvent paymentEvent, CancellationToken cancellationToken = default)
    {
        var paymentBaseUrl = _configuration["Payment:BaseUrl"]
            ?? "https://localhost:7000";

        var confirmUrl = $"{paymentBaseUrl}/api/payment/{paymentEvent.PaymentId}/confirm";
        var cancelUrl = $"{paymentBaseUrl}/api/payment/{paymentEvent.PaymentId}/cancel";

        var email = new EmailMessage
        {
            To = $"cliente-{paymentEvent.CustomerId}@fake.com",
            Subject = "Seu boleto fictício foi gerado",
            Body = $"""
                    Seu pedido {paymentEvent.OrderId} foi criado.

                    Valor: {paymentEvent.Amount}
                    Código do boleto: {paymentEvent.FakeBoletoCode}

                    Confirmar pagamento:
                    {confirmUrl}

                    Cancelar pagamento:
                    {cancelUrl}
                    """
        };

        await _emailSender.SendEmailAsync(email, cancellationToken);
    }
}