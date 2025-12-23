using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        public async Task<UserProgressResponse> GetUserCourseProgressAsync(int userId)
        {
            var routeId = await GetUserRouteIdAsync(userId);
            if (routeId == null)
                return EmptyProgress();

            var stages = await GetStagesForRouteAsync(routeId.Value);
            if (!stages.Any())
                return EmptyProgress();

            var allCourses = await GetCoursesForStagesAsync(stages);
            var completedCourseIds = await GetCompletedCourseIdsAsync(userId, allCourses);

            // Получаем ВСЕ курсы пользователя по этим этапам
            var userProgresses = await _onboardingContext.UserProgresses
                .Where(up => up.FkUserId == userId &&
                             allCourses.Select(c => c.Id).Contains(up.FkCourseId))
                .ToListAsync();

            var completedCourses = completedCourseIds.Count;
            var totalCourses = allCourses.Count;

            // Получаем статусы этапов из базы
            var stageStatuses = await _onboardingContext.UserOnboardingStageStatuses
                .Where(s => s.FkUserId == userId && stages.Select(st => st.Id).Contains(s.FkOnboardingStageId))
                .Select(s => new
                {
                    s.FkOnboardingStageId,
                    s.Status,
                    s.FactStartDate
                })
                .ToListAsync();

            var completedStages = 0;
            var stageProgress = new List<StageProgressItem>();

            foreach (var stage in stages)
            {
                var statusRecord = stageStatuses.FirstOrDefault(s => s.FkOnboardingStageId == stage.Id);
                var baseStatus = statusRecord?.Status ?? "current";

                // Получаем курсы этого этапа
                var stageCourses = allCourses.Where(c => c.StageId == stage.Id).ToList();
                var stageCourseIds = stageCourses.Select(c => c.Id).ToList();

                // Находим прогресс по курсам этого этапа
                var stageUserProgresses = userProgresses
                    .Where(up => stageCourseIds.Contains(up.FkCourseId))
                    .ToList();

                var completedInStage = stageUserProgresses.Count(up => up.Status == CourseStatuses.Completed);
                var inProgressInStage = stageUserProgresses.Count(up => up.Status == CourseStatuses.InProgress);
                var allCompleted = completedInStage == stageCourses.Count;

                string finalStatus;

                if (allCompleted)
                {
                    // Все курсы этапа завершены
                    finalStatus = "completed";
                    completedStages++;
                }
                else if (inProgressInStage > 0 || completedInStage > 0)
                {
                    // Есть хотя бы один начатый или завершенный курс в этапе
                    // Но не все курсы завершены
                    finalStatus = "failed";
                }
                else if (baseStatus == "completed")
                {
                    finalStatus = "completed";
                    completedStages++;
                }
                else
                {
                    // Нет прогресса по курсам
                    finalStatus = baseStatus switch
                    {
                        "completed" => "completed",
                        "failed" => "failed",
                        "Not Started" => "current",
                        _ => "current"
                    };
                }

                stageProgress.Add(new StageProgressItem
                {
                    StageId = stage.Id,
                    Status = finalStatus
                });
            }

            return new UserProgressResponse
            {
                TotalCourses = totalCourses,
                CompletedCourses = completedCourses,
                TotalStages = stages.Count,
                CompletedStages = completedStages,
                StageProgress = stageProgress
            };
        }

        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        private async Task<int?> GetUserRouteIdAsync(int userId)
        {
            return await _onboardingContext.UserOnboardingRouteStatuses
                .Where(r => r.FkUserId == userId)
                .Select(r => r.FkOnboardingRouteId)
                .FirstOrDefaultAsync();
        }

        private async Task<List<OnboardingStage>> GetStagesForRouteAsync(int routeId)
        {
            return await _onboardingContext.OnboardingStages
                .Where(s => s.FkOnbordingRouteId == routeId)
                .OrderBy(s => s.OrderIndex)
                .ToListAsync();
        }

        private async Task<List<CourseProjection>> GetCoursesForStagesAsync(List<OnboardingStage> stages)
        {
            var stageIds = stages.Select(s => (int?)s.Id).ToList();
            return await _onboardingContext.Courses
                .Where(c => stageIds.Contains(c.FkOnbordingStage))
                .OrderBy(c => c.OrderIndex)
                .Select(c => new CourseProjection
                {
                    Id = c.Id,
                    StageId = c.FkOnbordingStage,
                    Tests = c.Tests.Select(t => new TestProjection
                    {
                        Id = t.Id,
                        PassingScore = t.PassingScore,
                        Questions = t.Questions.Select(q => new QuestionProjection
                        {
                            Id = q.Id,
                            CorrectOptionIds = q.QuestionOptions
                                .Where(opt => opt.CorrectAnswer)
                                .Select(opt => opt.Id)
                                .ToList()
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();
        }

        private async Task<HashSet<int>> GetCompletedCourseIdsAsync(int userId, List<CourseProjection> courses)
        {
            var completedCourseIds = await _onboardingContext.UserProgresses
                .Where(up => up.FkUserId == userId &&
                             up.Status == CourseStatuses.Completed &&
                             courses.Select(c => c.Id).Contains(up.FkCourseId))
                .Select(up => up.FkCourseId)
                .ToListAsync();

            return new HashSet<int>(completedCourseIds);
        }

        private static UserProgressResponse EmptyProgress() => new()
        {
            TotalCourses = 0,
            CompletedCourses = 0,
            TotalStages = 0,
            CompletedStages = 0,
            StageProgress = new List<StageProgressItem>()
        };
    }
}
