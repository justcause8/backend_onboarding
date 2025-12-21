using backend_onboarding.Models.DTOs;
using backend_onboarding.Models.Entitie.DbOnboarding;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Services.Onboarding
{
    public partial class OnboardingService : IOnboardingService
    {
        public async Task<List<AnswerResponse>> GetUserAnswersByTestAsync(int userId, int testId)
        {
            return await _onboardingContext.Answers
                .Where(a => a.Fk1UserId == userId && a.Fk2Question.FkTestId == testId)
                .Select(a => new AnswerResponse
                {
                    Id = a.Id,
                    UserId = a.Fk1UserId,
                    QuestionId = a.Fk2QuestionId,
                    AnswerText = a.AnswerText,
                    CreatedAt = a.CreatedAt,
                    SelectedOptionIds = a.AnswerOptions
                        .Select(ao => ao.SelectedAnswerOption ?? 0)
                        .Where(id => id != 0)
                        .ToList()
                }).ToListAsync();
        }

        public async Task<int> SubmitAnswerAsync(UserAnswerRequest request)
        {
            var answer = new Answer
            {
                Fk1UserId = request.UserId,
                Fk2QuestionId = request.QuestionId,
                AnswerText = request.AnswerText ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                // Если есть выбранные опции, добавляем их
                AnswerOptions = request.SelectedOptionIds?.Select(optId => new AnswerOption
                {
                    SelectedAnswerOption = optId
                }).ToList() ?? new List<AnswerOption>()
            };

            _onboardingContext.Answers.Add(answer);
            await _onboardingContext.SaveChangesAsync();
            return answer.Id;
        }

        public async Task<bool> UpdateAnswerAsync(int answerId, UserAnswerRequest request, int currentUserId)
        {
            var answer = await _onboardingContext.Answers
                .Include(a => a.AnswerOptions)
                // Проверяем ID ответа И владельца одновременно
                .FirstOrDefaultAsync(a => a.Id == answerId && a.Fk1UserId == currentUserId);

            if (answer == null) return false;

            answer.AnswerText = request.AnswerText ?? string.Empty;

            _onboardingContext.AnswerOptions.RemoveRange(answer.AnswerOptions);

            if (request.SelectedOptionIds != null)
            {
                foreach (var optId in request.SelectedOptionIds)
                {
                    _onboardingContext.AnswerOptions.Add(new AnswerOption
                    {
                        FkAnswerId = answerId,
                        SelectedAnswerOption = optId
                    });
                }
            }

            await _onboardingContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAnswerAsync(int answerId, int currentUserId)
        {
            var answer = await _onboardingContext.Answers
                .FirstOrDefaultAsync(a => a.Id == answerId && a.Fk1UserId == currentUserId);

            if (answer == null) return false;

            _onboardingContext.Answers.Remove(answer);
            return await _onboardingContext.SaveChangesAsync() > 0;
        }
    }
}
