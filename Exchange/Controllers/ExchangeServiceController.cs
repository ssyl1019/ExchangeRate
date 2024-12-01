using Exchange.Services;
using Exchange.Types;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeServiceController : ControllerBase
    {

        private readonly IExchangeRateService _exchangeRateService;
        private readonly ILogger<ExchangeServiceController> _logger;

        public ExchangeServiceController(IExchangeRateService exchangeRateService, ILogger<ExchangeServiceController> logger)
        {
            _exchangeRateService = exchangeRateService;
            _logger = logger;
        }

        [HttpPost(Name = "ExchangeService")]
        public async Task<IActionResult> Post([FromBody] ExchangeRequest request)
        {

            if (request == null || request.Amount <= 0)
            {
                return BadRequest("Invalid request.");
            }

            try
            {
                var rate = await _exchangeRateService.GetExchangeRateAsync(request.InputCurrency, request.OutputCurrency);
                var value = request.Amount * rate;

                return Ok(new ExchangeResponse
                {
                    Amount = request.Amount,
                    InputCurrency = request.InputCurrency,
                    OutputCurrency = request.OutputCurrency,
                    Value = Math.Round(value, 2)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to fetch exchange rate due to following reason: {ex.Message}");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
