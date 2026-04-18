using NotificationService.Application.Ports;
using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Email;

public class FakeEmailSender : IEmailSender
{
    public Task SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("==== Sending Email ====");
        Console.WriteLine($"To: {message.To}");
        Console.WriteLine($"Subject: {message.Subject}");
        Console.WriteLine($"Body: ");
        Console.WriteLine(message.Body);
        Console.WriteLine("=======================");

        return Task.CompletedTask;
    }
}