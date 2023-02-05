using MagicVilla_API.Controllers;
using MagicVilla_API.Data;
using MagicVilla_API.Models;

namespace MagicVilla_API.Repository
{
    public class VillaRepository : Repository<Villa>, IVillaRepository
    {
        public VillaRepository(ILogger<VillaApiController> Logger, ApplicationDBContext db) : base(Logger, db)
        {
        }
    }
}
