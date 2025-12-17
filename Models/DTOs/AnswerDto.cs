namespace backend_onboarding.Models.DTOs
{
    public class UserAnswerRequest
    {
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public string? AnswerText { get; set; } // Для открытых вопросов
        public List<int>? SelectedOptionIds { get; set; } // Для закрытых/множественных (список ID из QuestionOption)
    }

    public class AnswerResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<int> SelectedOptionIds { get; set; } = new();
    }
}
