namespace backend_onboarding.Models.DTOs
{
    public class CreateUserInRIMSRequest
    {
        public string Login { get; set; } = null!;
        public string Caption { get; set; } = null!;
        public string Company { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string JobTitle { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string AppName { get; set; } = "RIMS_APP";
    }
}
