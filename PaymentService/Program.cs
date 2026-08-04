using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Events;
using PaymentService.Application.Ports;
using PaymentService.Application.UseCases;
using PaymentService.Infrastructure.Messaging;
using PaymentService.Infrastructure.Persistence;
using PaymentService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var allowFrontendPolicy = "_allowFrontendPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowFrontendPolicy,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000", "http://localhost:5173") 
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                      });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IEventBus, AzureServiceBusEventBus>();

builder.Services.AddScoped<CreatePaymentFromOrderUseCase>();
builder.Services.AddScoped<ConfirmPaymentUseCase>();
builder.Services.AddScoped<CancelPaymentUseCase>();

builder.Services.AddHostedService<OrderCreatedConsumer>();

var app = builder.Build();

app.UseCors(allowFrontendPolicy);
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    dbContext.Database.Migrate();
}

app.Run();