
using Exchange.Types.ExchangeRate;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace Exchange.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly ExchangeRateApiConfig _config;
        private static decimal? _cachedRate;
        private static DateTime? _lastUpdated;

        public ExchangeRateService(HttpClient httpClient, IOptions<ExchangeRateApiConfig> options)
        {
            _httpClient = httpClient;
            _config = options.Value;
        }

         public async Task<decimal> GetExchangeRateAsync(string baseCurrency, string targetCurrency)
        {
            if (_cachedRate.HasValue && _lastUpdated.HasValue && DateTime.Now - _lastUpdated < TimeSpan.FromMinutes(1))
            {
                return _cachedRate.Value;
            }

            var url = $"{_config.BaseUrl}{_config.ApiKey}/latest/{baseCurrency}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);

                if (response == null || !response.conversion_rates.TryGetValue(targetCurrency, out var rate))
                {
                    throw new Exception("Failed to fetch exchange rate.");
                }

                _cachedRate = rate;
                _lastUpdated = DateTime.Now;
                return rate;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch exchange rate due to following reason: {ex.Message}");
            }
        }
    }
}
