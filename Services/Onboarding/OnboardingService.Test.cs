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
                .Where(t => t.Fk1CourseId == courseId)
                .Select(t => new TestResponse
                {
                    Id = t.Id,
                    CourseId = t.Fk1CourseId,
                    AuthorId = t.Fk2User.Id,
                    Title = t.Title,
                    Description = t.Description,
                    PassingScore = t.PassingScore,
                    ResultsScore = 0, // Заглушка: результат индивидуален для пользователя
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
                    CourseId = t.Fk1CourseId,
                    AuthorId = t.Fk2UserId,
                    Title = t.Title,
                    Description = t.Description,
                    PassingScore = t.PassingScore,
                    ResultsScore = 0,
                    Status = t.Status,
                    QuestionsCount = t.Questions.Count,

                    Questions = t.Questions.Select(q => new QuestionResponse
                    {
                        Id = q.Id,
                        TestId = q.Fk1TestId,
                        QuestionTypeId = q.Fk2QuestionTypeId,
                        TypeName = q.Fk2QuestionType.NameQuestionType,
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

                        // ДОБАВЛЕНО: Получаем ответы именно этого пользователя на этот вопрос
                        UserAnswers = q.Answers
                            .Where(a => a.Fk1UserId == userId)
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
                Fk1CourseId = request.CourseId,
                Fk2UserId = request.AuthorId,
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

            test.Title = request.Title;
            test.Description = request.Description;
            test.PassingScore = request.PassingScore;
            test.Status = request.Status;
            test.Fk1CourseId = request.CourseId;

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTestAsync(int id)
        {
            var test = await _onboardingContext.Tests
                .Include(t => t.Questions)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null) return false;

            // Перед удалением теста нужно решить судьбу вопросов. 
            // Если в БД настроено каскадное удаление — это произойдет само.
            _onboardingContext.Tests.Remove(test);
            return await _onboardingContext.SaveChangesAsync() > 0;
        }
    }
}
