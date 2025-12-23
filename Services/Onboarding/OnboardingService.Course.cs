using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        // Статусы маршрута
        public static class RouteStatuses
        {
            public const string InProgress = "in_process";
            public const string Completed = "completed";
            public const string NotStarted = "not_started";
        }

        // Статусы курсов
        public static class CourseStatuses
        {
            public const string NotStarted = "not_started";
            public const string InProgress = "in_process";
            public const string Completed = "completed";
            public const string Failed = "failed";
        }

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
                .Include(c => c.Materials)
                .Include(c => c.Tests)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return false;

            if (course.Materials.Any())
                _onboardingContext.Materials.RemoveRange(course.Materials);

            if (course.Tests.Any())
                _onboardingContext.Tests.RemoveRange(course.Tests);

            _onboardingContext.Courses.Remove(course);

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

        // МЕТОД ДЛЯ НАЧАЛА КУРСА (кнопка "Пройти курс")
        public async Task<bool> StartCourseAsync(int userId, int courseId)
        {
            try
            {
                // 1. Проверяем существование курса
                var course = await _onboardingContext.Courses
                    .FirstOrDefaultAsync(c => c.Id == courseId);

                if (course == null)
                {
                    return false;
                }

                // 2. Проверяем, есть ли уже запись о прогрессе
                var userProgress = await _onboardingContext.UserProgresses
                    .FirstOrDefaultAsync(up => up.FkUserId == userId && up.FkCourseId == courseId);

                bool isNewOrUpdated = false;

                if (userProgress == null)
                {
                    userProgress = new UserProgress
                    {
                        FkUserId = userId,
                        FkCourseId = courseId,
                        Status = CourseStatuses.InProgress,
                    };
                    _onboardingContext.UserProgresses.Add(userProgress);
                    isNewOrUpdated = true;
                }
                else if (userProgress.Status != CourseStatuses.Completed &&
                        userProgress.Status != CourseStatuses.InProgress)
                {
                        userProgress.Status = CourseStatuses.InProgress;
                        isNewOrUpdated = true;
                }

                var stageId = await _onboardingContext.Courses
                    .Where(c => c.Id == courseId)
                    .Select(c => c.FkOnbordingStage)
                    .FirstOrDefaultAsync();

                if (stageId.HasValue)
                {
                    await UpdateStageStatusAsync(userId, stageId.Value, "current");
                }

                await UpdateRouteStatusBasedOnStagesAsync(userId);

                // Если были изменения - сохраняем
                if (isNewOrUpdated)
                {
                    var savedCount = await _onboardingContext.SaveChangesAsync();
                    return savedCount > 0;
                }

                // Если курс уже был начат - тоже возвращаем успех
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // МЕТОД ДЛЯ ЗАВЕРШЕНИЯ КУРСА (после теста)
        public async Task<bool> CompleteCourseAsync(int userId, int courseId)
        {
            var course = await _onboardingContext.Courses
                .Where(c => c.Id == courseId)
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
                .FirstOrDefaultAsync();

            if (course == null) return false;

            var isCourseCompleted = await IsCourseCompletedAsync(userId, course);
            if (!isCourseCompleted) return false;

            var userProgress = await _onboardingContext.UserProgresses
                .FirstOrDefaultAsync(up => up.FkUserId == userId && up.FkCourseId == courseId);

            if (userProgress == null)
            {
                userProgress = new UserProgress
                {
                    FkUserId = userId,
                    FkCourseId = courseId,
                    Status = CourseStatuses.Completed
                };
                _onboardingContext.UserProgresses.Add(userProgress);
            }
            else
            {
                userProgress.Status = CourseStatuses.Completed;
            }

            await _onboardingContext.SaveChangesAsync();

            var stageId = await _onboardingContext.Courses
                .Where(c => c.Id == courseId)
                .Select(c => c.FkOnbordingStage)
                .FirstOrDefaultAsync();

            if (stageId.HasValue)
            {
                await CheckAndUpdateStageCompletionAsync(userId, stageId.Value);
                await UpdateRouteStatusBasedOnStagesAsync(userId);
            }

            return true;
        }

        public async Task<bool> CheckAndUpdateCourseCompletionAsync(int userId, int courseId)
        {
            var course = await _onboardingContext.Courses
                .Where(c => c.Id == courseId)
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
                .FirstOrDefaultAsync();

            if (course == null) return false;

            var isCompleted = await IsCourseCompletedAsync(userId, course);

            if (isCompleted)
            {
                return await CompleteCourseAsync(userId, courseId);
            }

            return false;
        }
    }
}