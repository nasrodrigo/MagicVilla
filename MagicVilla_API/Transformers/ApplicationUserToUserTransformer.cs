using MagicVilla_API.Models;

namespace MagicVilla_API.Transformers
{
    public class ApplicationUserToUserTransformer : ITransformer<ApplicationUser, User>
    {
        public User CreateMap(ApplicationUser user)
        {
            return new User
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Password = user.PasswordHash
            };
        }
    }
}
