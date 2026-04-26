using NotificationService.Application.Ports;
using NotificationService.Domain.Models;
namespace NotificationService.Application.Services;

public class CustomerClient : ICustomerClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CustomerClient> _logger;

    public CustomerClient(IHttpClientFactory httpClientFactory, ILogger<CustomerClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<CustomerData?> GetCustomerEmailAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var httpClient = _httpClientFactory.CreateClient("CustomerService");
            var response = await httpClient.GetAsync($"/customer/email/{customerId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                return System.Text.Json.JsonSerializer.Deserialize<CustomerData>(json, options);
            }

            _logger.LogWarning($"Failed to get customer email. Status: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting customer email: {ex.Message}");
            return null;
        }
    }
}
