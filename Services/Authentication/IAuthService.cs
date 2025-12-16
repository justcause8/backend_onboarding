using backend_onboarding.Models.DTOs;

namespace backend_onboarding.Services.Authentication
{
    public interface IAuthService
    {
        Task<UserAuthRequest?> AuthenticateAsync();
        Task<UserAuthRequest?> AuthenticateByLoginAsync(string login); // Временный метод для аутентификации под чужим логином
    }
}