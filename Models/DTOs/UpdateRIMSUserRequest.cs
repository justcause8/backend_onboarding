namespace backend_onboarding.Models.DTOs
{
    public class UpdateRIMSUserRequest
    {
        public string? Caption { get; set; }
        public string? Company { get; set; }
        public string? Department { get; set; }
        public string? JobTitle { get; set; }

        // Данные для связанных таблиц
        public string? Email { get; set; }
        public string? Login { get; set; } // AccountName
        public string? Role { get; set; }
        public string? AppName { get; set; }
    }
}
