using MagicVilla_API.Models;
using MagicVilla_API.Models.DTOs;

namespace MagicVilla_API.Transformers
{
    public class VillaToVillaDTOTransformer: ITransformer<Villa, VillaDTO>
    {
       public VillaDTO CreateMap(Villa villa)
        {
            return new VillaDTO
            {
                Id = villa.Id,
                Name = villa.Name,
                Description = villa.Description,
                Rate = villa.Rate,
            };
        }
    }
}
