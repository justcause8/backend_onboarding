using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using backend_onboarding.Models.Entitie.DbOnboardingRIMS;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        public async Task<List<UserOnboardingResponse>> GetAllUsersAsync()
        {
            return await _onboardingContext.Users
                .Select(u => new UserOnboardingResponse
                {
                    Id = u.Id,
                    FullName = u.Name,
                    Email = u.Email,
                    Position = u.JobTitle ?? "Не указана",
                    Routes = u.UserOnboardingRouteStatuses.Select(urs => new UserRouteInfoDto
                    {
                        RouteId = urs.FkOnboardingRouteId,
                        Title = urs.FkOnboardingRoute.Title,
                        Status = urs.Status
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<UserOnboardingResponse?> GetUserOnboardingDataAsync(int userId)
        {
            return await _onboardingContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserOnboardingResponse
                {
                    Id = u.Id,
                    FullName = u.Name,
                    Email = u.Email,
                    Position = u.JobTitle ?? "Не указана",
                    Routes = u.UserOnboardingRouteStatuses.Select(urs => new UserRouteInfoDto
                    {
                        RouteId = urs.FkOnboardingRouteId,
                        Title = urs.FkOnboardingRoute.Title,
                        Status = urs.Status
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateOnboardingUserAsync(int userId, UpdateOnboardingUserRequest request)
        {
            var userToUpdate = await _onboardingContext.Users.FindAsync(userId);

            if (userToUpdate == null) return false;

            // Обновляем только те поля, которые пришли в запросе
            if (request.Name != null) userToUpdate.Name = request.Name;
            if (request.Department != null) userToUpdate.Department = request.Department;
            if (request.JobTitle != null) userToUpdate.JobTitle = request.JobTitle;
            if (request.Email != null) userToUpdate.Email = request.Email;
            if (request.Role != null) userToUpdate.Role = request.Role;
            if (request.Login != null) userToUpdate.Login = request.Login;

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOnboardingUserAsync(int userId)
        {
            var userToDelete = await _onboardingContext.Users.FindAsync(userId);

            if (userToDelete == null) return false;

            _onboardingContext.Users.Remove(userToDelete);
            await _onboardingContext.SaveChangesAsync();
            return true;
        }
    }
}
