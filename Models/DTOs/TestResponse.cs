namespace backend_onboarding.Models.DTOs
{
    public class TestResponse
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? PassingScore { get; set; }
        public decimal? ResultsScore { get; set; } // Заглушка
        public string Status { get; set; } = null!;
        public int QuestionsCount { get; set; }
        public List<QuestionResponse> Questions { get; set; } = new();
    }

    public class CreateTestRequest
    {
        public int? CourseId { get; set; }
        public int? AuthorId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? PassingScore { get; set; }
        public string? Status { get; set; }
    }
}
