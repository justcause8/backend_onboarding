namespace backend_onboarding.Models.DTOs
{
    public class UserOnboardingResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Position { get; set; } = null!;

        // Список маршрутов пользователя
        public List<UserRouteInfoDto> Routes { get; set; } = new List<UserRouteInfoDto>();
    }

    public class UserRouteInfoDto
    {
        public int RouteId { get; set; }
        public string Title { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

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
