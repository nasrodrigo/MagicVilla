using MagicVilla_API.Controllers;
using MagicVilla_API.Models;

namespace MagicVilla_API.Repository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        public VillaRepository(ApplicationDBContext db) : base(db)
        {
        }
    }
}
