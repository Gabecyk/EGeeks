using NotificationService.Application.Ports;
using NotificationService.Application.Services;
using NotificationService.Application.UseCases;
using NotificationService.Infrastructure.Email;
using NotificationService.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Uncomment the line below to use Azure Communication Services Email
builder.Services.AddScoped<IEmailSender, AzureEmailSender>();

// Comment the line below when using Azure Communication Services
// builder.Services.AddScoped<IEmailSender, FakeEmailSender>();

// HttpClient para comunicação com CustomerService
if (builder.Environment.IsDevelopment())
{
    // Em desenvolvimento, aceitar certificados auto-assinados
    builder.Services.AddHttpClient("CustomerService", client =>
    {
        var baseUrl = builder.Configuration["CustomerService:BaseUrl"] ?? "http://localhost:5209";
        client.BaseAddress = new Uri(baseUrl);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        return handler;
    });
}
else
{
    builder.Services.AddHttpClient("CustomerService", client =>
    {
        var baseUrl = builder.Configuration["CustomerService:BaseUrl"] ?? "https://localhost:5209";
        client.BaseAddress = new Uri(baseUrl);
    });
}

builder.Services.AddScoped<ICustomerClient, CustomerClient>();

builder.Services.AddScoped<HandlePaymentCreatedNotificationUseCase>();
builder.Services.AddScoped<HandlePaymentConfirmedNotificationUseCase>();
builder.Services.AddScoped<HandlePaymentCancelledNotificationUseCase>();

builder.Services.AddHostedService<NotificationConsumer>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();