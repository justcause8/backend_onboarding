namespace backend_onboarding.Models.DTOs
{
    public class QuestionResponse
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int QuestionTypeId { get; set; }
        public string TypeName { get; set; } = null!;
        public string TextQuestion { get; set; } = null!;
        public List<QuestionOptionDto> Options { get; set; } = new();
        public List<AnswerResponse> UserAnswers { get; set; } = new();
    }

    public class QuestionOptionDto
    {
        public int? Id { get; set; }
        public string Text { get; set; } = null!;
        public bool CorrectAnswer { get; set; }
        public int OrderIndex { get; set; }
    }

    public class CreateQuestionRequest
    {
        public int TestId { get; set; }
        public int QuestionTypeId { get; set; } // 1 - open, 2 - close, 3 - multiple
        public string TextQuestion { get; set; } = null!;
        public List<QuestionOptionDto> Options { get; set; } = new();
    }
}
