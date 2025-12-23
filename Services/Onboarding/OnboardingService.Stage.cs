using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using backend_onboarding.Models.Entitie.DbOnboardingRIMS;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        // Назначение пользователя на конкретный ЭТАП
        public async Task<bool> AssignUserToStageAsync(int userId, int stageId)
        {
            // Проверяем, нет ли уже такой записи, чтобы не дублировать
            var alreadyAssigned = await _onboardingContext.UserOnboardingStageStatuses
                .AnyAsync(s => s.FkUserId == userId && s.FkOnboardingStageId == stageId);

            if (alreadyAssigned) return true;

            var newStatus = new UserOnboardingStageStatus
            {
                FkUserId = userId,
                FkOnboardingStageId = stageId,
                Status = "current", // Используем единую систему статусов
                FactStartDate = DateTime.Now
            };

            _onboardingContext.UserOnboardingStageStatuses.Add(newStatus);
            return await _onboardingContext.SaveChangesAsync() > 0;
        }

        // Назначение пользователя на МАРШРУТ (авто-заполнение всех этапов)
        public async Task<bool> AssignUserToRouteAsync(int userId, int routeId)
        {
            // Получаем все ID этапов этого маршрута
            var stageIds = await _onboardingContext.OnboardingStages
                .Where(s => s.FkOnbordingRouteId == routeId)
                .Select(s => s.Id)
                .ToListAsync();

            if (!stageIds.Any()) return false;

            // Создаем начальные статусы для всех этапов
            foreach (var stageId in stageIds)
            {
                var existingStatus = await _onboardingContext.UserOnboardingStageStatuses
                    .FirstOrDefaultAsync(s => s.FkUserId == userId && s.FkOnboardingStageId == stageId);

                if (existingStatus == null)
                {
                    var newStatus = new UserOnboardingStageStatus
                    {
                        FkUserId = userId,
                        FkOnboardingStageId = stageId,
                        Status = "current", // Начальный статус
                        FactStartDate = DateTime.Now
                    };

                    _onboardingContext.UserOnboardingStageStatuses.Add(newStatus);
                }
            }

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

        // ПОЛУЧЕНИЕ
        public async Task<StageResponse?> GetStageByIdAsync(int stageId)
        {
            return await _onboardingContext.OnboardingStages
                .Where(s => s.Id == stageId)
                .Select(s => new StageResponse
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    OrderIndex = s.OrderIndex,

                    // Получаем сотрудников через таблицу UserOnboardingStageStatuses
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
                })
                .FirstOrDefaultAsync();
        }

        // СОЗДАНИЕ
        public async Task<bool> AddStagesToRouteAsync(AddStagesToRouteRequest request)
        {
            var route = await _onboardingContext.OnboardingRoutes
                .Include(r => r.UserOnboardingRouteStatuses)
                .FirstOrDefaultAsync(r => r.Id == request.RouteId);

            if (route == null) return false;

            var newStages = request.Stages.Select(s => new OnboardingStage
            {
                FkOnbordingRouteId = request.RouteId,
                Title = s.Title,
                Description = s.Description,
                OrderIndex = s.Order
            }).ToList();

            _onboardingContext.OnboardingStages.AddRange(newStages);
            await _onboardingContext.SaveChangesAsync();

            var currentParticipantIds = route.UserOnboardingRouteStatuses
                .Select(urs => urs.FkUserId)
                .ToList();

            if (currentParticipantIds.Any())
            {
                var newStageStatuses = new List<UserOnboardingStageStatus>();
                foreach (var stage in newStages)
                {
                    foreach (var userId in currentParticipantIds)
                    {
                        newStageStatuses.Add(new UserOnboardingStageStatus
                        {
                            FkUserId = userId,
                            FkOnboardingStageId = stage.Id,
                            Status = "current" // Используем единую систему
                        });
                    }
                }
                _onboardingContext.UserOnboardingStageStatuses.AddRange(newStageStatuses);
                await _onboardingContext.SaveChangesAsync();
            }

            return true;
        }

        // РЕДАКТИРОВАНИЕ
        public async Task<bool> UpdateStageAsync(int stageId, UpdateStageRequest request)
        {
            var stage = await _onboardingContext.OnboardingStages.FindAsync(stageId);
            if (stage == null) return false;

            // Обновляем только присланные поля
            if (request.Title != null) stage.Title = request.Title;
            if (request.Description != null) stage.Description = request.Description;
            if (request.Order.HasValue) stage.OrderIndex = request.Order.Value;

            return await _onboardingContext.SaveChangesAsync() > 0;
        }

        // УДАЛЕНИЕ
        public async Task<bool> DeleteStageAsync(int stageId)
        {
            var stage = await _onboardingContext.OnboardingStages.FindAsync(stageId);
            if (stage == null) return false;

            // Удаляем связанные статусы прохождения этого этапа, чтобы не нарушить связи
            var linkedStatuses = _onboardingContext.UserOnboardingStageStatuses
                .Where(s => s.FkOnboardingStageId == stageId);

            _onboardingContext.UserOnboardingStageStatuses.RemoveRange(linkedStatuses);
            _onboardingContext.OnboardingStages.Remove(stage);

            return await _onboardingContext.SaveChangesAsync() > 0;
        }
    }
}
