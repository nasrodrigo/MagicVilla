using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_API.Models.DTO
{
    public class LocalUserRoleDTO
    {
        public string Role { get; set; }
        public int LocalUserId { get; set; }
    }
}
