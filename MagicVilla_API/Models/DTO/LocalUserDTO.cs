namespace MagicVilla_API.Models.DTO
{
    public class LocalUserDTO
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public List<LocalUserRole> Roles { get; set; }
    }
}
