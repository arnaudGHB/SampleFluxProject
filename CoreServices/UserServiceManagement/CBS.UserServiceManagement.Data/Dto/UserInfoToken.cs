namespace CBS.UserServiceManagement.Data
{
    public class UserInfoToken
    {
        public string Id { get; set; } = string.Empty; // Initialisation pour éviter les nulls
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; }
    }
}