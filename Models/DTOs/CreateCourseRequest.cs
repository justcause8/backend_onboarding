namespace backend_onboarding.Models.DTOs
{
    public class CreateCourseRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int StageId { get; set; } // ID этапа (Fk2OnbordingStage)
        public int? AuthorId { get; set; } // ID создателя (Fk1UserId)
        public int OrderIndex { get; set; }
        public string Status { get; set; } = "Active";
    }
}
