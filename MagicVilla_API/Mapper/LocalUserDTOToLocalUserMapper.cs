using MagicVilla_API.Models.DTO;
using MagicVilla_API.Models;

namespace MagicVilla_API.Mapper
{
    public class LocalUserDTOToLocalUserMapper : IMapper<LocalUserDTO, LocalUser>
    {
        public LocalUser CreateMap(LocalUserDTO localUserDTO)
        {
            return new LocalUser
            {
                UserName = localUserDTO.UserName,
                Name = localUserDTO.Name,
                Password = localUserDTO.Password,
                Roles = localUserDTO.Roles,
            };
        }
    }
}
