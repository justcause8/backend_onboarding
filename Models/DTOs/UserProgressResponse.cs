namespace backend_onboarding.Models.DTOs
{
    public class UserProgressResponse
    {
        public int TotalCourses { get; set; }
        public int CompletedCourses { get; set; }
        public int TotalStages { get; set; }
        public int CompletedStages { get; set; }

        public List<StageProgressItem> StageProgress { get; set; } = new();
    }

    public class StageProgressItem
    {
        public int StageId { get; set; }
        public string Status { get; set; } // "completed", "failed", "current"
    }
}
