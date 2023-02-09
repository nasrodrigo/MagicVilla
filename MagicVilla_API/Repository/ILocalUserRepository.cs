using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;

namespace MagicVilla_API.Repository
{
    public interface ILocalUserRepository
    {
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<bool> isExistingUser(string userName);
        Task Register(UserDTO user);
    }
}
