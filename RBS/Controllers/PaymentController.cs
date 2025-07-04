using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBS.Models.Apple;
using RBS.Services.Interfaces;

namespace RBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IApplePaymentService _paymentService;
        public PaymentController(IApplePaymentService applePayment)
        {
            _paymentService = applePayment;
        }       

        [HttpGet("apple1")]
        public IActionResult StartAppleLogin()
        {
            var clientId = "com.your.bundle.id";
            var redirectUri = "https://localhost:7118/api/Auth/apple/callback";
            var scope = "name email";

            var url = $"https://appleid.apple.com/auth/authorize?" +
                      $"client_id={clientId}&" +
                      $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                      $"response_type=code&" +
                      $"response_mode=form_post&" +
                      $"scope={scope}";

            return Redirect(url);
        }

        [HttpPost("apple")]
        public async Task<IActionResult> AppleLogin([FromBody] AppleAuthRequest request)
        {
            var dataToReturn = _paymentService.AppleLogin(request);

            return Ok(dataToReturn);
        }
    }

}
