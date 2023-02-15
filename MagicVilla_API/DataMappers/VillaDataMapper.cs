using MagicVilla_API.Models;

namespace MagicVilla_API.DataMappers
{
    public class VillaDataMapper : DataMapper<Villa>, IVillaDataMapper
    {
        public VillaDataMapper(ApplicationDBContext db) : base(db) 
        {
        }
    }
}
