namespace backend_onboarding.Models.DTOs
{
    public class UpdateStageStatusRequest
    {
        public string Status { get; set; } // "completed", "failed", "current"
    }
}
