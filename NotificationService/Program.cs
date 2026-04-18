using NotificationService.Application.Ports;
using NotificationService.Application.UseCases;
using NotificationService.Infrastructure.Email;
using NotificationService.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEmailSender, FakeEmailSender>();

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