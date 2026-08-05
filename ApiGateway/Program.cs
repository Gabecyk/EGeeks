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

var app = builder.Build();

app.UseCors(allowFrontendPolicy);

app.MapReverseProxy();

app.Run();
