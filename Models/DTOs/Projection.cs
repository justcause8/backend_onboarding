namespace backend_onboarding.Models.DTOs
{
    public class CourseProjection
    {
        public int Id { get; set; }
        public int? StageId { get; set; }
        public List<TestProjection> Tests { get; set; } = new();
    }

    public class TestProjection
    {
        public int Id { get; set; }
        public decimal? PassingScore { get; set; }
        public List<QuestionProjection> Questions { get; set; } = new();
    }

    public class QuestionProjection
    {
        public int Id { get; set; }
        public List<int> CorrectOptionIds { get; set; } = new();
    }
}
