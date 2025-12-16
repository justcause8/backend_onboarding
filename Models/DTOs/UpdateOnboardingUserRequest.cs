namespace backend_onboarding.Models.DTOs
{
    public class UpdateOnboardingUserRequest
    {
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Login { get; set; }
    }
}
