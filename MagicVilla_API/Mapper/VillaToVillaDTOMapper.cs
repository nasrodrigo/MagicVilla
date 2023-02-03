using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;

namespace MagicVilla_API.Mapper
{
    public class VillaToVillaDTOMapper: IMapper<Villa, VillaDTO>
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
