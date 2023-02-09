namespace MagicVilla_API.Models.DTO
{
    public class LoginResponseDTO
    {
        public UserDTO User { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
    }
}
