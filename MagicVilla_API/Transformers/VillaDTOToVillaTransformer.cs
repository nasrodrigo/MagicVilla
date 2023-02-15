using MagicVilla_API.Models;
using MagicVilla_API.Models.DTOs;

namespace MagicVilla_API.Transformers
{
    public class VillaDTOToVillaTransformer: ITransformer<VillaDTO, Villa>
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
