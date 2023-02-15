using MagicVilla_API.Models;

namespace MagicVilla_API.Repositories
{
    public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
    {
        public VillaNumberRepository(ApplicationDBContext db) : base(db) 
        {
        }
    }
}
