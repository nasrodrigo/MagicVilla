namespace MagicVilla_API.Models.DTO
{
    public class LoginResponse
    {
        public LocalUser User { get; set; }
        public string Token { get; set; }
    }
}
