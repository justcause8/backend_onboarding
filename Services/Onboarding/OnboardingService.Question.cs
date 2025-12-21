using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        public async Task<List<QuestionResponse>> GetQuestionsByTestIdAsync(int testId)
        {
            return await _onboardingContext.Questions
                .Where(q => q.FkTestId == testId)
                .Select(q => new QuestionResponse
                {
                    Id = q.Id,
                    TestId = q.FkTestId,
                    QuestionTypeId = q.FkQuestionTypeId,
                    TypeName = q.FkQuestionType.NameQuestionType,
                    TextQuestion = q.TextQuestion,
                    Options = q.QuestionOptions.OrderBy(o => o.OrderIndex).Select(o => new QuestionOptionDto
                    {
                        Id = o.Id,
                        Text = o.Text,
                        CorrectAnswer = o.CorrectAnswer,
                        OrderIndex = o.OrderIndex
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<int> CreateQuestionAsync(CreateQuestionRequest request)
        {
            var question = new Question
            {
                FkTestId = request.TestId,
                FkQuestionTypeId = request.QuestionTypeId,
                TextQuestion = request.TextQuestion,
                QuestionOptions = request.Options.Select(o => new QuestionOption
                {
                    Text = o.Text,
                    CorrectAnswer = o.CorrectAnswer,
                    OrderIndex = o.OrderIndex
                }).ToList()
            };

            _onboardingContext.Questions.Add(question);
            await _onboardingContext.SaveChangesAsync();
            return question.Id;
        }

        public async Task<bool> UpdateQuestionAsync(int questionId, CreateQuestionRequest request)
        {
            var question = await _onboardingContext.Questions
                .Include(q => q.QuestionOptions)
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (question == null) return false;

            // 1. Обновляем текст вопроса, если он передан
            if (!string.IsNullOrEmpty(request.TextQuestion))
                question.TextQuestion = request.TextQuestion;

            // 2. Обновляем тип вопроса, если передан корректный ID (> 0)
            if (request.QuestionTypeId > 0)
            {
                // Проверка существования типа, чтобы избежать ошибки FK, которую мы видели ранее
                var typeExists = await _onboardingContext.QuestionTypes.AnyAsync(qt => qt.Id == request.QuestionTypeId);
                if (typeExists)
                    question.FkQuestionTypeId = request.QuestionTypeId;
            }

            // 3. Обновляем связь с тестом (если нужно перенести вопрос в другой тест)
            if (request.TestId > 0)
                question.FkTestId = request.TestId;

            // 4. Обновляем опции (только если список передан в запросе)
            // Проверяем на null, чтобы не удалить старые данные при частичном обновлении
            if (request.Options != null && request.Options.Any())
            {
                // Удаляем старые
                _onboardingContext.QuestionOptions.RemoveRange(question.QuestionOptions);

                // Добавляем новые
                question.QuestionOptions = request.Options.Select(o => new QuestionOption
                {
                    Text = o.Text,
                    CorrectAnswer = o.CorrectAnswer,
                    OrderIndex = o.OrderIndex
                }).ToList();
            }

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var question = await _onboardingContext.Questions.FindAsync(questionId);
            if (question == null) return false;

            _onboardingContext.Questions.Remove(question);
            return await _onboardingContext.SaveChangesAsync() > 0;
        }
    }
}
