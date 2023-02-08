using MagicVilla_API.Controllers;
using MagicVilla_API.Models;

namespace MagicVilla_API.Repository
{
    public class LocalUserRoleRepository : Repository<LocalUserRole>, ILocalUserRoleRepository
    {
        public LocalUserRoleRepository(ApplicationDBContext db) : base(db)
        {
        }
    }
}
