using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;

namespace MagicVilla_API.Repository
{
    public interface ILocalUserRepository : IRepository<LocalUser>
    {
        Task<LoginResponse> Login(LoginRequestDTO loginRequestDTO);
        Task<bool> isExistingUser(string userName);
        Task Register(LocalUser localUser);
    }
}
