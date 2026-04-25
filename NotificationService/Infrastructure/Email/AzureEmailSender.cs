using Azure;
using Azure.Communication.Email;
using NotificationService.Application.Ports;
using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Email;

public class AzureEmailSender : IEmailSender
{
    private readonly EmailClient _emailClient;
    private readonly string _senderEmail;
    private readonly ILogger<AzureEmailSender> _logger;

    public AzureEmailSender(IConfiguration configuration, ILogger<AzureEmailSender> logger)
    {
        _logger = logger;
        
        var connectionString = configuration["AzureCommunicationServices:ConnectionString"]
            ?? throw new InvalidOperationException("AzureCommunicationServices:ConnectionString is not configured");
        
        _senderEmail = configuration["AzureCommunicationServices:SenderEmail"]
            ?? throw new InvalidOperationException("AzureCommunicationServices:SenderEmail is not configured");

        _emailClient = new EmailClient(connectionString);
    }

    public async Task SendEmailAsync(NotificationService.Domain.Models.EmailMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailMessage = new Azure.Communication.Email.EmailMessage(
                senderAddress: _senderEmail,
                recipientAddress: message.To,
                content: new EmailContent(message.Subject)
                {
                    PlainText = message.Body,
                    Html = message.Body
                });

            var sendOperation = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage, cancellationToken);

            _logger.LogInformation($"Email sent successfully with result: {sendOperation.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to send email: {ex.Message}");
            throw;
        }
    }
}
