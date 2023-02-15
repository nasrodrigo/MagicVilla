using MagicVilla_API.Models;

namespace MagicVilla_API.Repositories
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        public VillaRepository(ApplicationDBContext db) : base(db) 
        {
        }
    }
}
