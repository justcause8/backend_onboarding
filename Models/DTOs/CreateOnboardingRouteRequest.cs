namespace backend_onboarding.Models.DTOs
{
    public class CreateOnboardingRouteRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? MentorId { get; set; }
        public List<int> UserIds { get; set; } = new List<int>();
    }
}
