using NotificationService.Domain.Models;

namespace NotificationService.Application.Ports;

public interface IEmailSender
{
    Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);
}