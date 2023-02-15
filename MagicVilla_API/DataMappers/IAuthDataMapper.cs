using MagicVilla_API.Models;

namespace MagicVilla_API.DataMappers
{
    public interface IAuthDataMapper
    {
        Task<bool> IsExistingUser(string userName);
        Task<LoginResponse> Login(LoginRequest loginRequest);
        Task Register(User user);
    }
}