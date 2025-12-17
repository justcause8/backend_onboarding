using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using backend_onboarding.Models.Entitie.DbOnboardingRIMS;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        public async Task<OnboardingRouteResponse?> GetOnboardingRouteByIdAsync(int routeId)
        {
            return await _onboardingContext.OnboardingRoutes
                .Where(r => r.Id == routeId)
                .Select(r => new OnboardingRouteResponse
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    MentorId = r.FkmentorId,

                    // Получаем сотрудников через таблицу UserOnboardingRouteStatuses
                    AssignedEmployees = r.UserOnboardingRouteStatuses.Select(urs => new UserShortResponse
                    {
                        Id = urs.FkUserId,
                        Name = urs.FkUser.Name,
                        Status = urs.Status
                    }).ToList(),

                    Stages = r.OnboardingStages
                        .OrderBy(s => s.OrderIndex)
                        .Select(s => new StageResponse
                        {
                            Id = s.Id,
                            Title = s.Title,
                            Description = s.Description,
                            OrderIndex = s.OrderIndex,
                            Courses = s.Courses
                                .OrderBy(c => c.OrderIndex)
                                .Select(c => new CourseShortResponse
                                {
                                    Id = c.Id,
                                    Title = c.Title,
                                    OrderIndex = c.OrderIndex
                                }).ToList()
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateOnboardingRouteAsync(CreateOnboardingRouteRequest request)
        {
            // 1. Создаем сам маршрут
            var newRoute = new OnboardingRoute
            {
                Title = request.Title,
                Description = request.Description,
                FkmentorId = request.MentorId,
                Status = "Created" // Начальный статус
            };

            _onboardingContext.OnboardingRoutes.Add(newRoute);
            await _onboardingContext.SaveChangesAsync();

            // 2. Привязываем пользователей к этому маршруту
            if (request.UserIds != null && request.UserIds.Any())
            {
                var userStatuses = request.UserIds.Select(userId => new UserOnboardingRouteStatus
                {
                    FkUserId = userId,
                    FkOnboardingRouteId = newRoute.Id,
                    Status = "Not Started",
                    // Даты начала и конца не заполняем по условию
                }).ToList();

                _onboardingContext.UserOnboardingRouteStatuses.AddRange(userStatuses);
                await _onboardingContext.SaveChangesAsync();
            }

            return newRoute.Id;
        }

        public async Task<bool> UpdateOnboardingRouteAsync(int routeId, CreateOnboardingRouteRequest request)
        {
            var route = await _onboardingContext.OnboardingRoutes
                .Include(r => r.UserOnboardingRouteStatuses)
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (route == null) return false;

            // 1. Обновляем основные поля
            route.Title = request.Title;
            route.Description = request.Description;
            route.FkmentorId = request.MentorId;

            // 2. Обновляем список пользователей (синхронизация)
            // Удаляем тех, кого больше нет в списке
            var usersToRemove = route.UserOnboardingRouteStatuses
                .Where(s => !request.UserIds.Contains(s.FkUserId))
                .ToList();
            _onboardingContext.UserOnboardingRouteStatuses.RemoveRange(usersToRemove);

            // Добавляем новых
            var existingUserIds = route.UserOnboardingRouteStatuses.Select(s => s.FkUserId).ToList();
            var newUsers = request.UserIds
                .Where(userId => !existingUserIds.Contains(userId))
                .Select(userId => new UserOnboardingRouteStatus
                {
                    FkUserId = userId,
                    FkOnboardingRouteId = routeId,
                    Status = "Not Started"
                });

            _onboardingContext.UserOnboardingRouteStatuses.AddRange(newUsers);

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

        // УДАЛЕНИЕ
        public async Task<bool> DeleteOnboardingRouteAsync(int routeId)
        {
            var route = await _onboardingContext.OnboardingRoutes.FindAsync(routeId);
            if (route == null) return false;

            // Удаляем связанные статусы пользователей (если не настроено Cascade Delete в БД)
            var linkedStatuses = _onboardingContext.UserOnboardingRouteStatuses
                .Where(s => s.FkOnboardingRouteId == routeId);

            _onboardingContext.UserOnboardingRouteStatuses.RemoveRange(linkedStatuses);

            // Удаляем сам маршрут
            _onboardingContext.OnboardingRoutes.Remove(route);

            await _onboardingContext.SaveChangesAsync();
            return true;
        }
    }
}
