namespace backend_onboarding.Models.DTOs
{
    public class UserShortResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    public class OnboardingRouteResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? MentorId { get; set; }
        public List<StageResponse> Stages { get; set; } = new List<StageResponse>();

        public List<UserShortResponse> AssignedEmployees { get; set; } = new();
    }

    public class StageResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int OrderIndex { get; set; }
        public List<CourseShortResponse> Courses { get; set; } = new();

        public List<UserShortResponse> AssignedEmployees { get; set; } = new();
    }

    public class CourseShortResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int OrderIndex { get; set; }
    }
}
