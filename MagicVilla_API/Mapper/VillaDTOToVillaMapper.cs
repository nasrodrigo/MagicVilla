using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;

namespace MagicVilla_API.Mapper
{
    public class VillaDTOToVillaMapper: IMapper<VillaDTO, Villa>
    {
       public Villa CreateMap(VillaDTO villaDTO)
        {
            return new Villa
            {
                Name = villaDTO.Name,
                Description = villaDTO.Description,
                Rate = villaDTO.Rate
            };
        }
    }
}
