using RBS.CORE;
using RBS.DTOs;
using RBS.Models.Apple;

namespace RBS.Services.Interfaces
{
    public interface IApplePaymentService
    {
        Task<ApiResponse<AppleTokenResponseDTO>> AppleLogin(AppleAuthRequest request);

    }
}
