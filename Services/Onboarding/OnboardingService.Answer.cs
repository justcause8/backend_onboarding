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


        public async Task<int> SubmitAnswerAsync(UserAnswerRequest request)
        {
            var answer = new Answer
            {
                Fk1UserId = request.UserId,
                Fk2QuestionId = request.QuestionId,
                AnswerText = request.AnswerText ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                AnswerOptions = request.SelectedOptionIds?.Select(optId => new AnswerOption
                {
                    SelectedAnswerOption = optId
                }).ToList() ?? new List<AnswerOption>()
            };

            _onboardingContext.Answers.Add(answer);
            await _onboardingContext.SaveChangesAsync();

            // Находим, к какому курсу относится этот вопрос
            var courseId = await _onboardingContext.Questions
                .Where(q => q.Id == request.QuestionId)
                .Select(q => q.FkTest.FkCourseId)
                .FirstOrDefaultAsync();

            // Находим, к какому этапу относится этот вопрос
            var stageId = await _onboardingContext.Questions
                .Where(q => q.Id == request.QuestionId)
                .Select(q => q.FkTest.FkCourse.FkOnbordingStage)
                .FirstOrDefaultAsync();

            if (courseId > 0)
            {
                // Проверяем, завершен ли курс после этого ответа
                await CheckAndUpdateCourseCompletionAsync(request.UserId, courseId);
            }

            if (stageId.HasValue)
            {
                // Пересчитываем статус этапа
                await RecalculateSingleStageStatusAsync(request.UserId, stageId.Value);
            }

            return answer.Id;
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

        // ЛОГИКА ПРОВЕРКИ ТЕСТОВ
        private async Task<bool> IsTestPassedAsync(int userId, TestProjection test)
        {
            // Получаем ВСЕ вопросы теста из БД
            var allQuestions = await _onboardingContext.Questions
                .Where(q => q.FkTestId == test.Id)
                .Select(q => new
                {
                    Id = q.Id,
                    QuestionTypeId = q.FkQuestionTypeId,
                    CorrectOptionIds = q.QuestionOptions
                        .Where(opt => opt.CorrectAnswer)
                        .Select(opt => opt.Id)
                        .ToList()
                })
                .ToListAsync();


            if (allQuestions.Count == 0)
            {
                return true;
            }

            // Проверяем, на все ли вопросы ответил пользователь
            var answeredQuestionIds = await _onboardingContext.Answers
                .Where(a => a.Fk1UserId == userId && allQuestions.Select(q => q.Id).Contains(a.Fk2QuestionId))
                .Select(a => a.Fk2QuestionId)
                .Distinct()
                .ToListAsync();


            // Если не на все вопросы ответили
            if (answeredQuestionIds.Count != allQuestions.Count)
            {
                var unanswered = allQuestions.Select(q => q.Id).Except(answeredQuestionIds).ToList();
                return false;
            }

            // Проверяем правильность ответов
            int correctAnswersCount = 0;
            int totalQuestions = allQuestions.Count;

            foreach (var question in allQuestions)
            {
                var userAnswer = await _onboardingContext.Answers
                    .Include(a => a.AnswerOptions)
                    .FirstOrDefaultAsync(a => a.Fk1UserId == userId && a.Fk2QuestionId == question.Id);

                if (userAnswer == null)
                {
                    return false;
                }

                bool isCorrect = false;

                // Для открытых вопросов (тип 1)
                if (question.QuestionTypeId == 1)
                {
                    if (!string.IsNullOrWhiteSpace(userAnswer.AnswerText))
                    {
                        isCorrect = true;
                    }
                }
                // Для вопросов с выбором (тип 2 - single, тип 3 - multiple)
                else if (question.QuestionTypeId == 2 || question.QuestionTypeId == 3)
                {
                    var selectedOptionIds = userAnswer.AnswerOptions
                        .Where(ao => ao.SelectedAnswerOption.HasValue)
                        .Select(ao => ao.SelectedAnswerOption.Value)
                        .ToList();

                    var correctSet = new HashSet<int>(question.CorrectOptionIds);
                    var selectedSet = new HashSet<int>(selectedOptionIds);

                    isCorrect = selectedSet.SetEquals(correctSet);
                }

                if (isCorrect)
                {
                    correctAnswersCount++;
                }
            }

            // Расчет процента правильных ответов
            decimal correctPercentage = (decimal)correctAnswersCount / totalQuestions * 100;
            bool isPassed = correctPercentage >= test.PassingScore;

            // Если passingScore = 0, тест всегда пройден (но проверяем, что на все вопросы ответили)
            if (test.PassingScore == 0)
            {
                return true;
            }

            return isPassed;
        }
    }
}
