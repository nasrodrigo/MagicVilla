using MagicVilla_API.Models;

namespace MagicVilla_API.DataMappers
{
    public class VillaNumberDataMapper : DataMapper<VillaNumber>, IVillaNumberDataMapper
    {
        public VillaNumberDataMapper(ApplicationDBContext db) : base(db)
        {
        }
    }
}
