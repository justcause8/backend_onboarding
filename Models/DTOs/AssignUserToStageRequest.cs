namespace backend_onboarding.Models.DTOs
{
    public class AssignUserToStageRequest
    {
        public int UserId { get; set; }
        public int StageId { get; set; }
    }

    public class AssignUserToRouteRequest
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
    }
}
