namespace MagicVilla_API.Models
{
    public class LoginResponse
    {
        public User? User { get; set; }
        public List<string>? Roles { get; set; }
        public string? Token { get; set; }
    }
}
