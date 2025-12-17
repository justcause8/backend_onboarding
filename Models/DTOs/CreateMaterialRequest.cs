namespace backend_onboarding.Models.DTOs
{
    public class CreateMaterialRequest
    {
        public int CourseId { get; set; }
        public string? UrlDocument { get; set; }
    }

    public class UpdateMaterialRequest
    {
        public string? UrlDocument { get; set; }
    }
}
