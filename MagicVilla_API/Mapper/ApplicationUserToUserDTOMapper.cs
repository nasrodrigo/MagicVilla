using MagicVilla_API.Models.DTO;
using MagicVilla_API.Models;

namespace MagicVilla_API.Mapper
{
    public class ApplicationUserToUserDTOMapper : IMapper<ApplicationUser, UserDTO>
    {
        public UserDTO CreateMap(ApplicationUser user)
        {
            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Password = user.PasswordHash
            };
        }
    }
}
