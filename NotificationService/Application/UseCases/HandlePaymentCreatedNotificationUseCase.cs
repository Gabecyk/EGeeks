using NotificationService.Application.Events;
using NotificationService.Application.Ports;
using NotificationService.Domain.Models;

namespace NotificationService.Application.UseCases;

public class HandlePaymentCreatedNotificationUseCase
{
    private readonly IEmailSender _emailSender;
    private readonly ICustomerClient _customerClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HandlePaymentCreatedNotificationUseCase> _logger;

    public HandlePaymentCreatedNotificationUseCase(
        IEmailSender emailSender,
        ICustomerClient customerClient,
        IConfiguration configuration,
        ILogger<HandlePaymentCreatedNotificationUseCase> logger)
    {
        _emailSender = emailSender;
        _customerClient = customerClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Execute(PaymentCreatedEvent paymentEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            var paymentBaseUrl = _configuration["Payment:BaseUrl"]
                ?? "https://localhost:5080";

            // Buscar dados do cliente
            var customer = await _customerClient.GetCustomerEmailAsync(paymentEvent.CustomerId, cancellationToken);

            if (customer == null || string.IsNullOrEmpty(customer.Email))
            {
                _logger.LogWarning($"Customer data not found for ID {paymentEvent.CustomerId}. Using fallback email.");
                throw new Exception($"Customer data not found for ID {paymentEvent.CustomerId}");
            }
            var customerEmail = customer.Email;
            var customerName = customer.Name;

            var confirmUrl = $"{paymentBaseUrl}/api/payment/{paymentEvent.PaymentId}/confirm";
            var cancelUrl = $"{paymentBaseUrl}/api/payment/{paymentEvent.PaymentId}/cancel";

            // Criar corpo de email HTML
            var htmlBody = GenerateEmailHTML(
                orderID: paymentEvent.OrderId.ToString(),
                customerName: customerName,
                totalAmount: paymentEvent.Amount.ToString("C"),
                boletoCode: paymentEvent.FakeBoletoCode,
                confirmUrl: confirmUrl,
                cancelUrl: cancelUrl
            );

            var email = new EmailMessage
            {
                To = customerEmail,
                Subject = "Seu boleto fictício foi gerado",
                Body = htmlBody
            };

            await _emailSender.SendEmailAsync(email, cancellationToken);
            _logger.LogInformation($"Notification email sent for payment {paymentEvent.PaymentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending notification email: {ex.Message}");
            throw;
        }
    }

    private string GenerateEmailHTML(string orderID, string customerName, string totalAmount, string boletoCode, string confirmUrl, string cancelUrl)
    {
        return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            padding: 30px;
        }}
        .greeting {{
            font-size: 16px;
            color: #333;
            margin-bottom: 20px;
        }}
        .info-box {{
            background-color: #f9f9f9;
            border-left: 4px solid #667eea;
            padding: 15px;
            margin: 20px 0;
            border-radius: 4px;
        }}
        .info-row {{
            display: flex;
            justify-content: space-between;
            padding: 10px 0;
            border-bottom: 1px solid #eee;
        }}
        .info-row:last-child {{
            border-bottom: none;
        }}
        .info-label {{
            font-weight: bold;
            color: #555;
        }}
        .info-value {{
            color: #333;
        }}
        .buttons {{
            display: flex;
            gap: 15px;
            margin-top: 30px;
            justify-content: center;
        }}
        .btn {{
            display: inline-block;
            padding: 12px 30px;
            border-radius: 4px;
            text-decoration: none;
            font-weight: bold;
            cursor: pointer;
            border: none;
            font-size: 14px;
            transition: all 0.3s ease;
        }}
        .btn-confirm {{
            background-color: #4CAF50;
            color: white;
        }}
        .btn-confirm:hover {{
            background-color: #45a049;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
        }}
        .btn-cancel {{
            background-color: #f44336;
            color: white;
        }}
        .btn-cancel:hover {{
            background-color: #da190b;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
        }}
        .footer {{
            background-color: #f5f5f5;
            padding: 20px;
            text-align: center;
            font-size: 12px;
            color: #999;
        }}
        .highlight {{
            color: #667eea;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>💳 Boleto Fictício Gerado</h1>
        </div>
        
        <div class=""content"">
            <div class=""greeting"">
                Olá <span class=""highlight"">{customerName}</span>,
            </div>
            
            <p>Seu pedido foi processado com sucesso! Segue abaixo os detalhes do pagamento:</p>
            
            <div class=""info-box"">
                <div class=""info-row"">
                    <span class=""info-label"">📋 ID do Pedido:</span>
                    <span class=""info-value"">{orderID}</span>
                </div>
                
                <div class=""info-row"">
                    <span class=""info-label"">💰 Valor Total:</span>
                    <span class=""info-value"">{totalAmount}</span>
                </div>
                
                <div class=""info-row"">
                    <span class=""info-label"">📄 Código do Boleto:</span>
                    <span class=""info-value"">{boletoCode}</span>
                </div>
            </div>
            
            <p style=""color: #666; font-size: 14px; margin-top: 20px;"">
                Por favor, confirme ou cancele o pagamento clicando nos botões abaixo:
            </p>
            
            <div class=""buttons"">
                <a href=""{confirmUrl}"" class=""btn btn-confirm"">✓ Confirmar Pagamento</a>
                <a href=""{cancelUrl}"" class=""btn btn-cancel"">✗ Cancelar Pagamento</a>
            </div>
            
            <p style=""color: #999; font-size: 12px; margin-top: 30px; font-style: italic;"">
                Este é um boleto fictício apenas para fins de demonstração. Clique em um dos botões acima para processar sua ação.
            </p>
        </div>
        
        <div class=""footer"">
            <p>E-Geeks | Serviço de Pagamento</p>
            <p>© 2026 Todos os direitos reservados</p>
        </div>
    </div>
</body>
</html>
";
    }
}