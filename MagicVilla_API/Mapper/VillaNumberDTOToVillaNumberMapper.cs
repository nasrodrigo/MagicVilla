using MagicVilla_API.Models.DTO;
using MagicVilla_API.Models;

namespace MagicVilla_API.Mapper
{
    public class VillaNumberDTOToVillaNumberMapper : IMapper<VillaNumberDTO, VillaNumber>
    {
        public VillaNumber CreateMap(VillaNumberDTO villaNumberDTO)
        {
            return new VillaNumber
            {
                VillaId = villaNumberDTO.VillaId,
                SpecialDetails = villaNumberDTO.SpecialDetails
            };
        }
    }
}
