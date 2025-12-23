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
                    // Выводим объект с данными ментора
                    Mentor = r.FkUser != null ? new UserShortResponse
                    {
                        Id = r.FkUser.Id,
                        Name = r.FkUser.Name,
                        // Для ментора статус в этом контексте обычно не выводится, 
                        // но можно подтянуть Role или Department, если нужно
                        Status = "Mentor"
                    } : null,

                    // Пользователи, назначенные на весь маршрут
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

                            AssignedEmployees = s.UserOnboardingStageStatuses.Select(uss => new UserShortResponse
                            {
                                Id = uss.FkUserId,
                                Name = uss.FkUser.Name,
                                Status = uss.Status
                            }).ToList(),

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
                FkUserId = request.MentorId,
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
                    Status = RouteStatuses.NotStarted,
                    // Даты начала и конца не заполняем по условию
                }).ToList();

                _onboardingContext.UserOnboardingRouteStatuses.AddRange(userStatuses);
                await _onboardingContext.SaveChangesAsync();
            }

            // 3. (Опционально) Авто-назначение на ЭТАПЫ этого маршрута
            var stages = await _onboardingContext.OnboardingStages
                .Where(s => s.FkOnbordingRouteId == newRoute.Id)
                .ToListAsync();

            foreach (var userId in request.UserIds)
            {
                var stageStatuses = stages.Select(s => new UserOnboardingStageStatus
                {
                    FkUserId = userId,
                    FkOnboardingStageId = s.Id,
                    Status = "current"
                });
                _onboardingContext.UserOnboardingStageStatuses.AddRange(stageStatuses);
            }
            await _onboardingContext.SaveChangesAsync();

            return newRoute.Id;
        }

        public async Task<bool> UpdateOnboardingRouteAsync(int routeId, UpdateOnboardingRouteRequest request)
        {
            var route = await _onboardingContext.OnboardingRoutes
                .Include(r => r.UserOnboardingRouteStatuses)
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (route == null) return false;

            // 1. Обновляем только те поля, которые переданы (не null)
            if (request.Title != null) route.Title = request.Title;
            if (request.Description != null) route.Description = request.Description;
            if (request.MentorId != null) route.FkUserId = request.MentorId;

            // 2. Синхронизация пользователей только если список передан
            if (request.UserIds != null)
            {
                // Удаляем тех, кого нет в новом списке
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
                        Status = "current"
                    });

                _onboardingContext.UserOnboardingRouteStatuses.AddRange(newUsers);
            }

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOnboardingRouteAsync(int routeId)
        {
            var route = await _onboardingContext.OnboardingRoutes
                .Include(r => r.OnboardingStages)
                .FirstOrDefaultAsync(r => r.Id == routeId);

            if (route == null) return false;

            // 1. Получаем ID всех этапов этого маршрута
            var stageIds = route.OnboardingStages.Select(s => s.Id).ToList();

            // 2. СОХРАНЯЕМ КУРСЫ: Отвязываем их от удаляемых этапов
            var coursesToUnlink = await _onboardingContext.Courses
                .Where(c => c.FkOnbordingStage != null && stageIds.Contains(c.FkOnbordingStage.Value))
                .ToListAsync();

            foreach (var course in coursesToUnlink)
            {
                course.FkOnbordingStage = null; // Курс остается в базе, но больше не привязан к этапу
            }

            // 3. Удаляем статусы пользователей по этим ЭТАПАМ
            var stageStatuses = _onboardingContext.UserOnboardingStageStatuses
                .Where(s => stageIds.Contains(s.FkOnboardingStageId));
            _onboardingContext.UserOnboardingStageStatuses.RemoveRange(stageStatuses);

            // 4. Удаляем сами ЭТАПЫ
            _onboardingContext.OnboardingStages.RemoveRange(route.OnboardingStages);

            // 5. Удаляем статусы пользователей по МАРШРУТУ
            var routeStatuses = _onboardingContext.UserOnboardingRouteStatuses
                .Where(s => s.FkOnboardingRouteId == routeId);
            _onboardingContext.UserOnboardingRouteStatuses.RemoveRange(routeStatuses);

            // 6. Удаляем сам МАРШРУТ
            _onboardingContext.OnboardingRoutes.Remove(route);

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

        // Получение маршрута для пользователя
        public async Task<int?> GetMyRouteIdAsync(int userId)
        {
            var routeStatus = await _onboardingContext.UserOnboardingRouteStatuses
                .Where(r => r.FkUserId == userId)
                .Select(r => r.FkOnboardingRouteId)
                .FirstOrDefaultAsync();

            return routeStatus == default(int) ? (int?)null : routeStatus;
        }

        // ОБНОВЛЕНИЕ СТАТУСА МАРШРУТА
        private async Task UpdateRouteStatusBasedOnStagesAsync(int userId)
        {
            var routeId = await GetUserRouteIdAsync(userId);
            if (!routeId.HasValue) return;

            var routeStatus = await _onboardingContext.UserOnboardingRouteStatuses
                .FirstOrDefaultAsync(r => r.FkUserId == userId && r.FkOnboardingRouteId == routeId.Value);

            if (routeStatus == null) return;

            var stages = await GetStagesForRouteAsync(routeId.Value);
            var stageStatuses = await _onboardingContext.UserOnboardingStageStatuses
                .Where(s => s.FkUserId == userId && stages.Select(st => st.Id).Contains(s.FkOnboardingStageId))
                .ToListAsync();

            // Проверяем, есть ли хотя бы один этап со статусом "current" и датой начала
            bool hasStartedStage = stageStatuses.Any(s =>
                s.Status == "current" && s.FactStartDate.HasValue);

            // Проверяем, завершен ли последний этап
            var lastStage = stages.OrderByDescending(s => s.OrderIndex).FirstOrDefault();
            bool isLastStageCompleted = false;

            if (lastStage != null)
            {
                var lastStageStatus = stageStatuses.FirstOrDefault(s => s.FkOnboardingStageId == lastStage.Id);
                isLastStageCompleted = lastStageStatus != null &&
                                      lastStageStatus.Status == "completed" &&
                                      lastStageStatus.FactEndDate.HasValue;
            }

            string newStatus;
            if (isLastStageCompleted)
            {
                newStatus = RouteStatuses.Completed;
                routeStatus.FactEndDate ??= DateTime.Now;
            }
            else if (hasStartedStage)
            {
                newStatus = RouteStatuses.InProgress;
                routeStatus.FactStartDate ??= DateTime.Now;
            }
            else
            {
                newStatus = RouteStatuses.NotStarted;
            }

            if (routeStatus.Status != newStatus)
            {
                routeStatus.Status = newStatus;
                await _onboardingContext.SaveChangesAsync();
            }
        }
    }
}
