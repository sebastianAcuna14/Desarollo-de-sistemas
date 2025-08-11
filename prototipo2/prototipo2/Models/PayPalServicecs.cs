using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace prototipo2.Services
{
    public class PayPalService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PayPalService> _logger;
        private string _accessToken;
        private DateTime _tokenExpires;
        private readonly object _tokenLock = new object();

        public PayPalService(HttpClient httpClient, IConfiguration configuration, ILogger<PayPalService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Configuración base del HttpClient
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            // Verificación doble con bloqueo para evitar race conditions
            if (!string.IsNullOrEmpty(_accessToken) && _tokenExpires > DateTime.UtcNow.AddMinutes(1))
            {
                return _accessToken;
            }

            lock (_tokenLock)
            {
                if (!string.IsNullOrEmpty(_accessToken) && _tokenExpires > DateTime.UtcNow.AddMinutes(1))
                {
                    return _accessToken;
                }
            }

            var clientId = _configuration["PayPal:ClientId"];
            var secret = _configuration["PayPal:ClientSecret"];
            var env = _configuration["PayPal:Environment"] ?? "sandbox";

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(secret))
            {
                _logger.LogError("PayPal credentials not configured");
                throw new InvalidOperationException("PayPal credentials not configured");
            }

            var baseUrl = env == "live" ? "https://api-m.paypal.com" : "https://api-m.sandbox.paypal.com";

            try
            {
                var authToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{secret}"));

                using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v1/oauth2/token");
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);
                request.Content = new StringContent("grant_type=client_credentials",
                    System.Text.Encoding.UTF8,
                    "application/x-www-form-urlencoded");

                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"PayPal token request failed: {response.StatusCode} - {errorContent}");
                    response.EnsureSuccessStatusCode(); // Esto lanzará una excepción
                }

                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var json = await JsonDocument.ParseAsync(responseStream);

                lock (_tokenLock)
                {
                    _accessToken = json.RootElement.GetProperty("access_token").GetString();
                    int expiresIn = json.RootElement.GetProperty("expires_in").GetInt32();
                    _tokenExpires = DateTime.UtcNow.AddSeconds(expiresIn);
                }

                return _accessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PayPal access token");
                throw;
            }
        }

        public async Task<bool> ValidarOrdenAsync(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                _logger.LogWarning("Order ID cannot be null or empty");
                return false;
            }

            try
            {
                var accessToken = await GetAccessTokenAsync();
                var env = _configuration["PayPal:Environment"] ?? "sandbox";
                var baseUrl = env == "live" ? "https://api-m.paypal.com" : "https://api-m.sandbox.paypal.com";

                using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/v2/checkout/orders/{orderId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"PayPal order validation failed for {orderId}. Status: {response.StatusCode}");
                    return false;
                }

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var json = await JsonDocument.ParseAsync(contentStream);

                var status = json.RootElement.GetProperty("status").GetString();
                var isValid = status == "COMPLETED";

                if (!isValid)
                {
                    _logger.LogInformation($"PayPal order {orderId} status: {status}");
                }

                return isValid;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, $"HTTP error validating PayPal order {orderId}");
                return false;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, $"JSON parsing error validating PayPal order {orderId}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error validating PayPal order {orderId}");
                return false;
            }
        }
    }
}