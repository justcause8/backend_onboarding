using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboardingRIMS;
namespace backend_onboarding.Services.Rims
{
    public interface IRimsService
    {
        Task<User> CreateUserAsync(CreateUserInRIMSRequest request);
        Task<bool> UpdateUserAsync(int userId, UpdateRIMSUserRequest request);
        Task<bool> DeleteUserAsync(int userId);
    }
}