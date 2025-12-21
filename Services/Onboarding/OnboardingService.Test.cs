using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        public async Task<List<TestResponse>> GetTestsByCourseIdAsync(int courseId)
        {
            return await _onboardingContext.Tests
                .Where(t => t.FkCourseId == courseId)
                .Select(t => new TestResponse
                {
                    Id = t.Id,
                    CourseId = t.FkCourseId,
                    AuthorId = t.FkUserId,
                    Title = t.Title,
                    Description = t.Description,
                    PassingScore = t.PassingScore,
                    ResultsScore = 0,
                    Status = t.Status,
                    QuestionsCount = t.Questions.Count
                }).ToListAsync();
        }

        public async Task<TestResponse?> GetTestByIdAsync(int id, int userId)
        {
            return await _onboardingContext.Tests
                .Where(t => t.Id == id)
                .Select(t => new TestResponse
                {
                    Id = t.Id,
                    CourseId = t.FkCourseId,
                    AuthorId = t.FkUserId,
                    Title = t.Title,
                    Description = t.Description,
                    PassingScore = t.PassingScore,
                    Status = t.Status,
                    QuestionsCount = t.Questions.Count,

                    Questions = t.Questions.Select(q => new QuestionResponse
                    {
                        Id = q.Id,
                        TestId = q.FkTestId,
                        QuestionTypeId = q.FkQuestionTypeId,
                        TypeName = q.FkQuestionType.NameQuestionType,
                        TextQuestion = q.TextQuestion,

                        Options = q.QuestionOptions
                            .OrderBy(o => o.OrderIndex)
                            .Select(o => new QuestionOptionDto
                            {
                                Id = o.Id,
                                Text = o.Text,
                                CorrectAnswer = o.CorrectAnswer,
                                OrderIndex = o.OrderIndex
                            }).ToList(),

                        UserAnswers = q.Answers
                            .Select(a => new AnswerResponse
                            {
                                Id = a.Id,
                                UserId = a.Fk1UserId,
                                QuestionId = a.Fk2QuestionId,
                                AnswerText = a.AnswerText,
                                CreatedAt = a.CreatedAt,
                                SelectedOptionIds = a.AnswerOptions
                                    .Select(ao => ao.SelectedAnswerOption ?? 0)
                                    .ToList()
                            }).ToList()
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<int> CreateTestAsync(CreateTestRequest request)
        {
            var test = new Test
            {
                FkCourseId = request.CourseId ?? 0,
                FkUserId = request.AuthorId ?? 0,
                Title = request.Title,
                Description = request.Description,
                PassingScore = request.PassingScore,
                Status = request.Status,
                ResultsScore = 0 // Инициализация заглушки
            };

            _onboardingContext.Tests.Add(test);
            await _onboardingContext.SaveChangesAsync();
            return test.Id;
        }

        public async Task<bool> UpdateTestAsync(int id, CreateTestRequest request)
        {
            var test = await _onboardingContext.Tests.FindAsync(id);
            if (test == null) return false;

            // Обновляем только если значения переданы (не null)
            if (!string.IsNullOrEmpty(request.Title))
                test.Title = request.Title;

            if (request.Description != null)
                test.Description = request.Description;

            if (request.PassingScore.HasValue)
                test.PassingScore = request.PassingScore.Value;

            if (!string.IsNullOrEmpty(request.Status))
                test.Status = request.Status;

            if (request.CourseId.HasValue)
                test.FkCourseId = request.CourseId.Value;

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTestAsync(int id)
        {
            var test = await _onboardingContext.Tests
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null) return false;

            _onboardingContext.Tests.Remove(test);
            return await _onboardingContext.SaveChangesAsync() > 0;
        }
    }
}
