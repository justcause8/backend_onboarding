using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using backend_onboarding.Models.Entitie.DbOnboardingRIMS;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        // 1. Назначение пользователя на конкретный ЭТАП
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
                Status = "Not Started", // Начальный статус
                FactStartDate = DateTime.Now
            };

            _onboardingContext.UserOnboardingStageStatuses.Add(newStatus);
            return await _onboardingContext.SaveChangesAsync() > 0;
        }

        // 2. Назначение пользователя на МАРШРУТ (авто-заполнение всех этапов)
        public async Task<bool> AssignUserToRouteAsync(int userId, int routeId)
        {
            // Получаем все ID этапов этого маршрута
            var stageIds = await _onboardingContext.OnboardingStages
                .Where(s => s.Fk1OnbordingRouteId == routeId)
                .Select(s => s.Id)
                .ToListAsync();

            if (!stageIds.Any()) return false;

            foreach (var stageId in stageIds)
            {
                await AssignUserToStageAsync(userId, stageId);
            }

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
            var routeExists = await _onboardingContext.OnboardingRoutes.AnyAsync(r => r.Id == request.RouteId);
            if (!routeExists) return false;

            var stages = request.Stages.Select(s => new OnboardingStage
            {
                Fk1OnbordingRouteId = request.RouteId,
                Title = s.Title,
                Description = s.Description,
                OrderIndex = s.Order // Используем Order из DTO для OrderIndex
            }).ToList();

            _onboardingContext.OnboardingStages.AddRange(stages);
            return await _onboardingContext.SaveChangesAsync() > 0;
        }

        // РЕДАКТИРОВАНИЕ
        public async Task<bool> UpdateStageAsync(int stageId, StageDto request)
        {
            var stage = await _onboardingContext.OnboardingStages.FindAsync(stageId);
            if (stage == null) return false;

            stage.Title = request.Title;
            stage.Description = request.Description;
            stage.OrderIndex = request.Order;

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
