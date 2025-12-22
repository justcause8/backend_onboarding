using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        // ПОЛУЧЕНИЕ КУРСА
        public async Task<CourseFullResponse?> GetCourseByIdAsync(int courseId)
        {
            return await _onboardingContext.Courses
                .Where(c => c.Id == courseId)
                .Select(c => new CourseFullResponse
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    OrderIndex = c.OrderIndex,
                    Status = c.Status,
                    StageId = c.FkOnbordingStage,
                    Materials = c.Materials.Select(m => new MaterialResponse
                    {
                        Id = m.Id,
                        UrlDocument = m.UrlDocument
                    }).ToList(),
                    Tests = c.Tests.Select(t => new TestShortResponse
                    {
                        Id = t.Id,
                        Title = t.Title,
                        PassingScore = t.PassingScore
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        // ПОЛУЧЕНИЕ ВСЕХ КУРСОВ
        public async Task<List<CourseFullResponse>> GetAllCoursesFullAsync()
        {
            return await _onboardingContext.Courses
                .OrderBy(c => c.OrderIndex)
                .Select(c => new CourseFullResponse
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    OrderIndex = c.OrderIndex,
                    Status = c.Status,
                    StageId = c.FkOnbordingStage,
                    Materials = c.Materials.Select(m => new MaterialResponse
                    {
                        Id = m.Id,
                        UrlDocument = m.UrlDocument
                    }).ToList(),
                    Tests = c.Tests.Select(t => new TestShortResponse
                    {
                        Id = t.Id,
                        Title = t.Title,
                        PassingScore = t.PassingScore
                    }).ToList()
                })
                .ToListAsync();
        }

        // СОЗДАНИЕ КУРСА
        public async Task<int> CreateCourseAsync(CreateCourseRequest request)
        {
            // Проверяем, существует ли этап, к которому привязываем курс
            var stageExists = await _onboardingContext.OnboardingStages
                .AnyAsync(s => s.Id == request.StageId);

            if (!stageExists) throw new Exception("Этап не найден");

            var newCourse = new Course
            {
                Title = request.Title,
                Description = request.Description,
                FkOnbordingStage = request.StageId,
                OrderIndex = request.OrderIndex,
                Status = request.Status
            };

            _onboardingContext.Courses.Add(newCourse);
            await _onboardingContext.SaveChangesAsync();

            return newCourse.Id;
        }

        // РЕДАКТИРОВАНИЕ КУРСА
        public async Task<bool> UpdateCourseAsync(int courseId, UpdateCourseRequest request)
        {
            var course = await _onboardingContext.Courses.FindAsync(courseId);
            if (course == null) return false;

            if (request.Title != null) course.Title = request.Title;
            if (request.Description != null) course.Description = request.Description;
            if (request.OrderIndex.HasValue) course.OrderIndex = request.OrderIndex.Value;
            if (request.Status != null) course.Status = request.Status;
            if (request.StageId.HasValue) course.FkOnbordingStage = request.StageId.Value;

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

        // УДАЛЕНИЕ КУРСА
        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var course = await _onboardingContext.Courses
                .Include(c => c.Materials) // Подгружаем связанные материалы
                .Include(c => c.Tests)     // Подгружаем связанные тесты
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return false;

            // 1. Удаляем материалы, привязанные к курсу
            if (course.Materials.Any())
                _onboardingContext.Materials.RemoveRange(course.Materials);

            // 2. Удаляем тесты, привязанные к курсу
            if (course.Tests.Any())
                _onboardingContext.Tests.RemoveRange(course.Tests);

            // 3. Удаляем сам курс
            _onboardingContext.Courses.Remove(course);

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

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

            var completedCourses = completedCourseIds.Count;
            var totalCourses = allCourses.Count;
            var completedStages = CountCompletedStages(stages, allCourses, completedCourseIds);
            var totalStages = stages.Count;

            var stageProgress = new List<StageProgressItem>();

            // Получим ID всех вопросов по курсам пользователя — чтобы определить, "начаты" ли курсы
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
                    stageProgress.Add(new StageProgressItem { StageId = stage.Id, Status = "completed" });
                    continue;
                }

                var completedInStage = stageCourseIds.Count(id => completedCourseIds.Contains(id));

                if (completedInStage == stageCourseIds.Count)
                {
                    stageProgress.Add(new StageProgressItem { StageId = stage.Id, Status = "completed" });
                }
                else
                {
                    // Проверяем: есть ли хоть один ответ по курсам этого этапа?
                    var stageQuestionIds = allCourses
                        .Where(c => c.StageId == stage.Id)
                        .SelectMany(c => c.Tests.SelectMany(t => t.Questions.Select(q => q.Id)))
                        .ToList();

                    bool hasAnyAnswer = stageQuestionIds.Any(qId => answeredQuestionIds.Contains(qId));

                    if (hasAnyAnswer)
                    {
                        stageProgress.Add(new StageProgressItem { StageId = stage.Id, Status = "failed" });
                    }
                    else
                    {
                        stageProgress.Add(new StageProgressItem { StageId = stage.Id, Status = "current" });
                    }
                }
            }

            return new UserProgressResponse
            {
                TotalCourses = totalCourses,
                CompletedCourses = completedCourses,
                TotalStages = totalStages,
                CompletedStages = completedStages,
                StageProgress = stageProgress
            };
        }

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
            var completed = new HashSet<int>();
            foreach (var course in courses)
            {
                if (await IsCourseCompletedAsync(userId, course))
                {
                    completed.Add(course.Id);
                }
            }
            return completed;
        }

        private async Task<bool> IsCourseCompletedAsync(int userId, CourseProjection course)
        {
            if (!course.Tests.Any())
                return true;

            foreach (var test in course.Tests)
            {
                if (!await IsTestPassedAsync(userId, test))
                    return false;
            }
            return true;
        }

        private async Task<bool> IsTestPassedAsync(int userId, TestProjection test)
        {
            var questionIds = test.Questions.Select(q => q.Id).ToList();
            if (!questionIds.Any())
                return true;

            var answeredQuestionIds = await _onboardingContext.Answers
                .Where(a => a.Fk1UserId == userId && questionIds.Contains(a.Fk2QuestionId))
                .Select(a => a.Fk2QuestionId)
                .ToListAsync();

            if (answeredQuestionIds.Count != questionIds.Count)
                return false;

            decimal score = 0;
            int totalQuestions = questionIds.Count;

            foreach (var question in test.Questions)
            {
                var userAnswer = await _onboardingContext.Answers
                    .FirstOrDefaultAsync(a => a.Fk1UserId == userId && a.Fk2QuestionId == question.Id);
                if (userAnswer == null)
                    return false;

                var selectedOptionIds = await _onboardingContext.AnswerOptions
                    .Where(ao => ao.FkAnswerId == userAnswer.Id && ao.SelectedAnswerOption.HasValue)
                    .Select(ao => ao.SelectedAnswerOption.Value)
                    .ToListAsync();

                var correctSet = new HashSet<int>(question.CorrectOptionIds);
                var selectedSet = new HashSet<int>(selectedOptionIds);

                if (selectedSet.SetEquals(correctSet))
                    score++;
            }

            var requiredScore = test.PassingScore * totalQuestions;
            return score >= requiredScore;
        }

        private int CountCompletedStages(
            List<OnboardingStage> stages,
            List<CourseProjection> allCourses,
            HashSet<int> completedCourseIds)
        {
            int completed = 0;
            foreach (var stage in stages)
            {
                var stageCourseIds = allCourses
                    .Where(c => c.StageId == stage.Id)
                    .Select(c => c.Id)
                    .ToList();

                if (!stageCourseIds.Any() || stageCourseIds.All(id => completedCourseIds.Contains(id)))
                {
                    completed++;
                }
            }
            return completed;
        }

        private static UserProgressResponse EmptyProgress() => new()
        {
            TotalCourses = 0,
            CompletedCourses = 0,
            TotalStages = 0,
            CompletedStages = 0
        };

        private class CourseProjection
        {
            public int Id { get; set; }
            public int? StageId { get; set; }
            public List<TestProjection> Tests { get; set; } = new();
        }

        private class TestProjection
        {
            public int Id { get; set; }
            public decimal? PassingScore { get; set; }
            public List<QuestionProjection> Questions { get; set; } = new();
        }

        private class QuestionProjection
        {
            public int Id { get; set; }
            public List<int> CorrectOptionIds { get; set; } = new();
        }
    }
}
