using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        private async Task<List<StageStatusRecord>> GetStageStatusesForUserAsync(int userId, List<int> stageIds)
        {
            return await _onboardingContext.UserOnboardingStageStatuses
                .Where(s => s.FkUserId == userId && stageIds.Contains(s.FkOnboardingStageId))
                .Select(s => new StageStatusRecord
                {
                    StageId = s.FkOnboardingStageId,
                    Status = s.Status
                })
                .ToListAsync();
        }

        private class StageStatusRecord
        {
            public int StageId { get; set; }
            public string Status { get; set; } = "current"; // "completed", "failed", "current"
        }

        // Метод для обновления статуса этапа
        public async Task<bool> UpdateStageStatusAsync(int userId, int stageId, string status)
        {
            var stageStatus = await _onboardingContext.UserOnboardingStageStatuses
                .FirstOrDefaultAsync(s => s.FkUserId == userId && s.FkOnboardingStageId == stageId);

            if (stageStatus == null)
            {
                stageStatus = new UserOnboardingStageStatus
                {
                    FkUserId = userId,
                    FkOnboardingStageId = stageId,
                    Status = status,
                    FactStartDate = DateTime.Now
                };

                if (status == "completed")
                {
                    stageStatus.FactEndDate = DateTime.Now;
                }

                _onboardingContext.UserOnboardingStageStatuses.Add(stageStatus);
            }
            else
            {
                // Если статус меняется на "current" и нет даты начала
                if (status == "current" && !stageStatus.FactStartDate.HasValue)
                {
                    stageStatus.FactStartDate = DateTime.Now;
                }

                stageStatus.Status = status;

                if (status == "completed" && !stageStatus.FactEndDate.HasValue)
                {
                    stageStatus.FactEndDate = DateTime.Now;
                }
                else if (status != "completed")
                {
                    stageStatus.FactEndDate = null;
                }
            }

            var result = await _onboardingContext.SaveChangesAsync() > 0;

            // После обновления статуса этапа обновляем статус маршрута
            if (result)
            {
                await UpdateRouteStatusBasedOnStagesAsync(userId);
            }

            return result;
        }

        // Метод для пересчета статуса только одного этапа
        private async Task RecalculateSingleStageStatusAsync(int userId, int stageId)
        {
            var routeId = await GetUserRouteIdAsync(userId);
            if (routeId == null) return;

            var stage = await _onboardingContext.OnboardingStages
                .FirstOrDefaultAsync(s => s.Id == stageId && s.FkOnbordingRouteId == routeId.Value);

            if (stage == null) return;

            var allCourses = await _onboardingContext.Courses
                .Where(c => c.FkOnbordingStage == stageId)
                .Select(c => new CourseProjection
                {
                    Id = c.Id,
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

            var completedCourseIds = await GetCompletedCourseIdsAsync(userId, allCourses);

            var stageCourseIds = allCourses.Select(c => c.Id).ToList();

            if (!stageCourseIds.Any())
            {
                await UpdateStageStatusAsync(userId, stageId, "completed");
                return;
            }

            var completedInStage = stageCourseIds.Count(id => completedCourseIds.Contains(id));

            if (completedInStage == stageCourseIds.Count)
            {
                await UpdateStageStatusAsync(userId, stageId, "completed");
            }
            else
            {
                // Проверяем, есть ли хоть один ответ по курсам этого этапа
                var stageQuestionIds = allCourses
                    .SelectMany(c => c.Tests.SelectMany(t => t.Questions.Select(q => q.Id)))
                    .ToList();

                var answeredQuestionIds = new HashSet<int>();
                if (stageQuestionIds.Any())
                {
                    var userAnswers = await _onboardingContext.Answers
                        .Where(a => a.Fk1UserId == userId && stageQuestionIds.Contains(a.Fk2QuestionId))
                        .Select(a => a.Fk2QuestionId)
                        .ToListAsync();
                    answeredQuestionIds = new HashSet<int>(userAnswers);
                }

                bool hasAnyAnswer = stageQuestionIds.Any(qId => answeredQuestionIds.Contains(qId));
                await UpdateStageStatusAsync(userId, stageId, hasAnyAnswer ? "failed" : "current");
            }
        }

        // Автоматический пересчет статусов этапов для пользователя
        public async Task RecalculateStageStatusesAsync(int userId)
        {
            var routeId = await GetUserRouteIdAsync(userId);
            if (routeId == null) return;

            var stages = await GetStagesForRouteAsync(routeId.Value);
            if (!stages.Any()) return;

            var allCourses = await GetCoursesForStagesAsync(stages);
            var completedCourseIds = await GetCompletedCourseIdsAsync(userId, allCourses);

            var allQuestionIds = allCourses
                .SelectMany(c => c.Tests.SelectMany(t => t.Questions.Select(q => q.Id)))
                .ToList();

            var answeredQuestionIds = new HashSet<int>();
            if (allQuestionIds.Any())
            {
                var userAnswers = await _onboardingContext.Answers
                    .Where(a => a.Fk1UserId == userId && allQuestionIds.Contains(a.Fk2QuestionId))
                    .Select(a => a.Fk2QuestionId)
                    .ToListAsync();
                answeredQuestionIds = new HashSet<int>(userAnswers);
            }

            foreach (var stage in stages)
            {
                var stageCourseIds = allCourses
                    .Where(c => c.StageId == stage.Id)
                    .Select(c => c.Id)
                    .ToList();

                if (!stageCourseIds.Any())
                {
                    // Если в этапе нет курсов, он считается завершенным
                    await UpdateStageStatusAsync(userId, stage.Id, "completed");
                    continue;
                }

                var completedInStage = stageCourseIds.Count(id => completedCourseIds.Contains(id));

                if (completedInStage == stageCourseIds.Count)
                {
                    // Все курсы этапа завершены
                    await UpdateStageStatusAsync(userId, stage.Id, "completed");
                }
                else
                {
                    // Есть незавершенные курсы
                    var stageQuestionIds = allCourses
                        .Where(c => c.StageId == stage.Id)
                        .SelectMany(c => c.Tests.SelectMany(t => t.Questions.Select(q => q.Id)))
                        .ToList();

                    bool hasAnyAnswer = stageQuestionIds.Any(qId => answeredQuestionIds.Contains(qId));

                    // Если есть хотя бы один ответ - этап начат, но не завершен (failed)
                    // Если нет ответов - этап текущий
                    await UpdateStageStatusAsync(userId, stage.Id, hasAnyAnswer ? "failed" : "current");
                }
            }
        }

        public async Task MigrateOldStatusesAsync()
        {
            // Конвертируем старые статусы "Not Started" в "current"
            var oldStatuses = await _onboardingContext.UserOnboardingStageStatuses
                .Where(s => s.Status == "Not Started")
                .ToListAsync();

            foreach (var status in oldStatuses)
            {
                status.Status = "current";
            }

            await _onboardingContext.SaveChangesAsync();
        }

        public async Task<UserProgressResponse> GetUserCourseProgressWithRecalculationAsync(int userId)
        {
            // Сначала пересчитываем статусы
            await RecalculateStageStatusesAsync(userId);

            // Затем возвращаем актуальный прогресс
            return await GetUserCourseProgressAsync(userId);
        }

        // МЕТОДЫ ОБНОВЛЕНИЯ СТАТУСОВ
        private async Task CheckAndUpdateStageCompletionAsync(int userId, int stageId)
        {
            var stageCourseIds = await _onboardingContext.Courses
                .Where(c => c.FkOnbordingStage == stageId)
                .Select(c => c.Id)
                .ToListAsync();

            // Если в этапе нет курсов - он автоматически завершен
            if (!stageCourseIds.Any())
            {
                await UpdateStageStatusAsync(userId, stageId, "completed");
                return;
            }

            var completedCoursesCount = await _onboardingContext.UserProgresses
                .CountAsync(up => up.FkUserId == userId &&
                                 stageCourseIds.Contains(up.FkCourseId) &&
                                 up.Status == CourseStatuses.Completed);

            if (completedCoursesCount == stageCourseIds.Count)
            {
                // Все курсы завершены
                await UpdateStageStatusAsync(userId, stageId, "completed");
            }
            else
            {
                // Проверяем, есть ли хоть один ответ в этом этапе
                var stageQuestionIds = await _onboardingContext.Courses
                    .Where(c => c.FkOnbordingStage == stageId)
                    .SelectMany(c => c.Tests.SelectMany(t => t.Questions.Select(q => q.Id)))
                    .ToListAsync();

                if (stageQuestionIds.Any())
                {
                    var answeredCount = await _onboardingContext.Answers
                        .CountAsync(a => a.Fk1UserId == userId && stageQuestionIds.Contains(a.Fk2QuestionId));

                    // Если есть ответы, но не все курсы завершены - этап провален
                    await UpdateStageStatusAsync(userId, stageId, answeredCount > 0 ? "failed" : "current");
                }
                else
                {
                    // Если нет вопросов вообще
                    await UpdateStageStatusAsync(userId, stageId, "current");
                }
            }
        }
    }
}
