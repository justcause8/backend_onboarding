using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using Microsoft.EntityFrameworkCore;

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
    }
}
