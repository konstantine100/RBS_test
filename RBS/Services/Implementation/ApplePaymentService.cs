using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text.Json;
using RBS.CORE;
using RBS.DTOs;
using RBS.Models.Apple;
using RBS.Services.Interfaces;

namespace RBS.Services.Implementation
{
    public class ApplePaymentService : IApplePaymentService
    {
        private readonly HttpClient _httpClient;
        public ApplePaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ApiResponse<AppleTokenResponseDTO>> AppleLogin(AppleAuthRequest request)
        {
            var appleclientId = "your-client-id-from-apple";
            var clientSecret = "your-signed-jwt-secret"; // use JWT signed with your Apple key

            var parameters = new Dictionary<string, string>
        {
            {"client_id", appleclientId },
            {"client_secret", clientSecret },
            {"code", request.Code },
            {"grant_type", "authorization_code" },
            {"redirect_uri", request.RedirectUri }
        };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync("https://appleid.apple.com/auth/token", content);

            if (!response.IsSuccessStatusCode)
            {

                return null;
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<AppleTokenResponse>(responseString);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenResponse.id_token);
            var payload = JsonSerializer.Deserialize<AppleIdTokenPayload>(JsonSerializer.Serialize(jwtToken.Payload));

            // Save or find user in DB
            var userEmail = payload.email;
            var userAppleId = payload.sub;

            var SuccessResponse = ApiResponseFactory.CreateResponse(
                StatusCodes.Status200OK,
                "Login successful",
                data: new AppleTokenResponseDTO
                {
                    Email = userEmail,
                    AppleId = userAppleId,
                    AccessToken = tokenResponse.access_token
                });


            return new ApiResponse<AppleTokenResponseDTO>
            {
                Data = SuccessResponse.Data,
                Status = SuccessResponse.Status,
                Message = SuccessResponse.Message
            };

        }


    }
}

