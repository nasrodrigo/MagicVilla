using MagicVilla_API.Models.DTO;
using MagicVilla_API.Models;

namespace MagicVilla_API.Mapper
{
    public class UserDTOToApplicationUserMapper : IMapper<UserDTO, ApplicationUser>
    {
        public ApplicationUser CreateMap(UserDTO user)
        {
            return new ApplicationUser
            {
                UserName = user.UserName,
                Email = user.UserName,
                NormalizedEmail = user.UserName.ToLower(),
                Name = user.Name
            };
        }
    }
}
