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
                .Where(q => q.Fk1TestId == testId)
                .Select(q => new QuestionResponse
                {
                    Id = q.Id,
                    TestId = q.Fk1TestId,
                    QuestionTypeId = q.Fk2QuestionTypeId,
                    TypeName = q.Fk2QuestionType.NameQuestionType,
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
                Fk1TestId = request.TestId,
                Fk2QuestionTypeId = request.QuestionTypeId,
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

            // Обновляем основные поля
            question.TextQuestion = request.TextQuestion;
            question.Fk2QuestionTypeId = request.QuestionTypeId;

            // Простой способ обновления опций: удаляем старые, добавляем новые
            _onboardingContext.QuestionOptions.RemoveRange(question.QuestionOptions);

            question.QuestionOptions = request.Options.Select(o => new QuestionOption
            {
                Text = o.Text,
                CorrectAnswer = o.CorrectAnswer,
                OrderIndex = o.OrderIndex
            }).ToList();

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
