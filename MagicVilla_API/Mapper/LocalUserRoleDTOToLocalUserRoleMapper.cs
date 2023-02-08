using MagicVilla_API.Models.DTO;
using MagicVilla_API.Models;

namespace MagicVilla_API.Mapper
{
    public class LocalUserRoleDTOToLocalUserRoleMapper : IMapper<LocalUserRoleDTO, LocalUserRole>
    {
        public LocalUserRole CreateMap(LocalUserRoleDTO localUserRoleDTO)
        {
            return new LocalUserRole
            {
               Role = localUserRoleDTO.Role,
               LocalUserId = localUserRoleDTO.LocalUserId
            };
        }
    }
}
