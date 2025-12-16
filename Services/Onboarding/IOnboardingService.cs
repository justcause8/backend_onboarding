using backend_onboarding.Models.DTOs;
namespace backend_onboarding.Services.Onboarding
{
    public interface IOnboardingService
    {
        Task<bool> UpdateOnboardingUserAsync(int userId, UpdateOnboardingUserRequest request);
        Task<bool> DeleteOnboardingUserAsync(int userId);
    }
}