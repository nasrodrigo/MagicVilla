using MagicVilla_API.Models;

namespace MagicVilla_API.Transformers
{
    public class UserToApplicationUserTransformer : ITransformer<User, ApplicationUser>
    {
        public ApplicationUser CreateMap(User user)
        {
            return new ApplicationUser
            {
                UserName = user.UserName,
                Email = user.UserName,
                NormalizedEmail = user.UserName!.ToLower(),
                Name = user.Name
            };
        }
    }
}
