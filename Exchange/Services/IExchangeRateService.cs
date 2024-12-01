namespace Exchange.Services
{
    public interface IExchangeRateService
    {
        Task<decimal> GetExchangeRateAsync(string baseCurrency, string targetCurrency);
    }
}
