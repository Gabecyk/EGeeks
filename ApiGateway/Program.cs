using Microsoft.AspNetCore.Diagnostics.HealthChecks;

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

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Cada downstream service adicionado aqui aparece no /health agregado do gateway.
// Ao criar o /health de um novo servico, registre-o com AddUrlGroup igual ao storeservice abaixo.
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri("http://authservice:8000/health"), name: "authservice")
    .AddUrlGroup(new Uri("http://orderservice:8082/health"), name: "orderservice")
    .AddUrlGroup(new Uri("http://storeservice:8080/health"), name: "storeservice")
    .AddUrlGroup(new Uri("http://paymentservice:8081/health"), name: "paymentservice")
    .AddUrlGroup(new Uri("http://customerservice:8084/health"), name: "customerservice");

var app = builder.Build();

app.UseCors(allowFrontendPolicy);

app.MapReverseProxy();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var payload = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        };
        await context.Response.WriteAsJsonAsync(payload);
    }
});

app.Run();
