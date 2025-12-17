namespace backend_onboarding.Models.DTOs
{
    // DTO для ответа WhoAmI
    public class UserAuthRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
    }
}
